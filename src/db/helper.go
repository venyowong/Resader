package db

import (
	"database/sql"
)

func ToList[T comparable](rows *sql.Rows, f func(*sql.Rows) T) []T {
	list := []T{}
	var d T
	for rows.Next() {
		t := f(rows)
		if t != d {
			list = append(list, t)
		}
	}
	return list
}
