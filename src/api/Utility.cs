using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Dapper;

namespace Resader.Api;

public static class Utility
{
    public static void MakeDapperMapping(string namspace)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes().Where(t => t.FullName.Contains(namspace)))
            {
                var map = new CustomPropertyTypeMap(type, (t, columnName) => t.GetProperties().FirstOrDefault(
                    prop => GetDescriptionFromAttribute(prop) == columnName || prop.Name.ToLower().Equals(columnName.ToLower())));
                SqlMapper.SetTypeMap(type, map);
            }
        }
    }

    /// <summary>
    /// ���� MakeDapperMapping(string namspace) �� Startup.Configure �����ŵ���
    /// <para>��������֮��ʱ��������ִ�е�����£����ܻ�δ���ӳ�䣬�ᵼ�²����ֶε� Column �����޷���Ч</para>
    /// <para>��˶����ṩ�÷��������������ݿ����֮ǰ�ȶԲ��� Type ����ӳ��</para>
    /// </summary>
    /// <param name="types"></param>
    public static void MakeDapperMapping(params Type[] types)
    {
        foreach (var type in types)
        {
            var map = new CustomPropertyTypeMap(type, (t, columnName) => t.GetProperties().FirstOrDefault(
                        prop => GetDescriptionFromAttribute(prop) == columnName || prop.Name.ToLower().Equals(columnName.ToLower())));
            SqlMapper.SetTypeMap(type, map);
        }
    }

    private static string GetDescriptionFromAttribute(MemberInfo member)
    {
        if (member == null) return null;

        var attrib = (ColumnAttribute)Attribute.GetCustomAttribute(member, typeof(ColumnAttribute), false);
        return attrib == null ? null : attrib.Name;
    }
}
