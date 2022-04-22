package helper

type IOrderKey interface {
	Key() string
}

func Select[T, V any](list []T, f func(t T) V) []V {
	result := []V{}
	for _, item := range list {
		result = append(result, f(item))
	}
	return result
}

func Order[T IOrderKey](list []T, f func(t T) float64, asc bool) []T {
	m := map[string]float64{}
	n := len(list) // 完全二叉树节点数
	for n > 1 {
		i := n/2 - 1 // 最后一棵子树的父节点下标
		for j := i; j >= 0; j-- {
			l := getOrderValue(m, list[j*2+1], f) // 左节点
			swap := j*2 + 1
			g := l
			if j*2+2 < n { //存在右节点
				r := getOrderValue(m, list[j*2+2], f)
				if asc { // 升序，最大堆
					if r > g {
						swap = j*2 + 2
						g = r
					}
				} else {
					if r < g {
						swap = j*2 + 2
						g = r
					}
				}
			}
			p := getOrderValue(m, list[j], f) // 父节点
			if asc {                          // 升序，最大堆
				if g > p { // 子节点大于父节点
					item := list[j]
					list[j] = list[swap]
					list[swap] = item
				}
			} else {
				if g < p { // 子节点小于父节点
					item := list[j]
					list[j] = list[swap]
					list[swap] = item
				}
			}
		}

		// 堆顶置换到堆尾
		item := list[0]
		list[0] = list[n-1]
		list[n-1] = item
		n--
	}
	return list
}

func getOrderValue[T IOrderKey](m map[string]float64, t T, f func(t T) float64) float64 {
	key := t.Key()
	v, e := m[key]
	if e {
		return v
	}

	v = f(t)
	m[key] = v
	return v
}
