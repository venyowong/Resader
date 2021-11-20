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
    /// 由于 MakeDapperMapping(string namspace) 在 Startup.Configure 的最后才调用
    /// <para>服务启动之后定时任务立即执行的情况下，可能还未完成映射，会导致部分字段的 Column 特性无法生效</para>
    /// <para>因此额外提供该方法，可用于数据库操作之前先对部分 Type 进行映射</para>
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
