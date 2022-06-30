using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DEVGIS.CsharpLibs
{
    public abstract class BLLClass
    {
        private enum ModelType
        {
            //值类型
            Struct,
            Enum,

            //引用类型
            String,
            Object,
            Else
        }

        private ModelType GetModelType(Type modelType)
        {
            if (modelType.IsEnum)//值类型
            {
                return ModelType.Enum;
            }
            if (modelType.IsValueType)//值类型
            {
                return ModelType.Struct;
            }
            else if (modelType == typeof(string))//引用类型 特殊类型处理
            {
                return ModelType.String;
            }
            else if (modelType == typeof(object))//引用类型 特殊类型处理
            {
                return ModelType.Object;
            }
            else//引用类型
            {
                return ModelType.Else;
            }
        }

        public T DataRowToModel<T>(DataRow row)
        {
            T model;
            Type type = typeof(T);
            ModelType modelType = GetModelType(type);
            switch (modelType)
            {
                case ModelType.Struct://值类型
                    {
                        model = default(T);
                        if (row[0] != null)
                            model = (T)row[0];
                    }
                    break;
                case ModelType.Enum://值类型
                    {
                        model = default(T);
                        if (row[0] != null)
                        {
                            Type fiType = row[0].GetType();
                            if (fiType == typeof(int))
                            {
                                model = (T)row[0];
                            }
                            else if (fiType == typeof(string))
                            {
                                model = (T)Enum.Parse(typeof(T), row[0].ToString());
                            }
                        }
                    }
                    break;
                case ModelType.String://引用类型 c#对string也当做值类型处理
                    {
                        model = default(T);
                        if (row[0] != null)
                            model = (T)row[0];
                    }
                    break;
                case ModelType.Object://引用类型 直接返回第一行第一列的值
                    {
                        model = default(T);
                        if (row[0] != null)
                            model = (T)row[0];
                    }
                    break;
                case ModelType.Else://引用类型
                    {
                        model = System.Activator.CreateInstance<T>();//引用类型 必须对泛型实例化
                        #region MyRegion

                        //遍历model每一个属性并赋值DataRow对应的列
                        foreach (var pi in typeof(T).GetProperties())
                        {
                            if (row.Table.Columns.Contains(pi.Name) && row[pi.Name] != null)
                            {
                                try
                                {
                                    pi.SetValue(model, Convert.ChangeType(row[pi.Name], pi.PropertyType),null);
                                }
                                catch (System.InvalidCastException)
                                { }
                            }
                        }

                        //遍历model每一个并赋值DataRow对应的列
                        foreach (var field in typeof(T).GetFields())
                        {
                            if (row.Table.Columns.Contains(field.Name) && row[field.Name] != null)
                            {
                                try
                                {
                                    field.SetValue(model, Convert.ChangeType(row[field.Name], field.FieldType));
                                }
                                catch (System.InvalidCastException)
                                { }
                            }
                        }
                        #endregion
                    }
                    break;
                default:
                    model = default(T);
                    break;
            }

            return model;
        }

        public List<T> DataTableToList<T>(DataTable table)
        {
            List<T> list = new List<T>();
            foreach (DataRow item in table.Rows)
            {
                list.Add(DataRowToModel<T>(item));
            }
            return list;
        }

        public string GetID()
        {
            return Guid.NewGuid().ToString("N").ToUpper();
        }
    }
}