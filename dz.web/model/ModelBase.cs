using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace dz.web.model
{
    public class ModelBase : IModel, IDisposable,ICloneable
    {
        /// <summary>
        /// 获取模型主键
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetPrimaryKeys()
        {
            return new string[] { "ID" };
        }

        /// <summary>
        /// 获取标识列
        /// </summary>
        /// <returns></returns>
        public virtual string GetIdentityKey()
        {
            return  "ID" ;
        }

        /// <summary>
        /// 获取模型对应的表名，默认为模型名称
        /// </summary>
        /// <returns></returns>
        public virtual string GetTableName()
        {
            return this.GetType().Name;
        }

        /// <summary>
        /// 实现IDisposable的唯一接口，用于销毁对象
        /// </summary>
        public virtual void Dispose()
        {

        }

        /// <summary>
        /// 获取查询语句
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public virtual string GetSelectSQL(int n=-1)
        {
            var propertys = this.GetType().GetProperties();

            List<string> listPropertyString = new List<string>();
            foreach(var p in propertys){
                listPropertyString.Add("["+p.Name+"]");
            }

            if (n >= 0)
                return string.Format(" SELECT TOP {2} {0} from [{1}] ", string.Join(",", listPropertyString.ToArray()), this.GetTableName(), n);
            else
                return string.Format(" SELECT {0} from [{1}] ", string.Join(",", listPropertyString.ToArray()), this.GetTableName());
        }

        /// <summary>
        /// 获取插入语句
        /// </summary>
        /// <returns></returns>
        public virtual string GetInsertSQL()
        {
            var propertys = this.GetType().GetProperties();

            List<string> listPropertyString = new List<string>();
            List<string> listValueString = new List<string>();
            foreach (var p in propertys)
            {
                if (p.Name == this.GetIdentityKey()) continue;

                listPropertyString.Add("[" + p.Name + "]");
                listValueString.Add("@"+p.Name);
            }

            return string.Format(" INSERT INTO [{0}]({1}) VALUES( {2} ) ", this.GetTableName(), string.Join(",", listPropertyString.ToArray()), string.Join(",", listValueString.ToArray()));
        }


        /// <summary>
        /// 获取修改语句
        /// </summary>
        /// <returns></returns>
        public virtual string GetUpdateSQL()
        {
            var propertys = this.GetType().GetProperties();
            List<string> listPropertyValueString = new List<string>();
            
            foreach (var p in propertys)
            {
                if (p.Name == this.GetIdentityKey()) continue;

                listPropertyValueString.Add("[" + p.Name + "] = @"+p.Name);
            }

            List<string> listWhereString = new List<string>();
            if (string.IsNullOrWhiteSpace(this.GetIdentityKey()))
            {
                foreach (var filed in this.GetPrimaryKeys())
                {
                    listWhereString.Add("[" + filed + "] = @" + filed);
                }
            }
            else
            {
                listWhereString.Add("[" + this.GetIdentityKey() + "] = @" + this.GetIdentityKey());
            }

            return string.Format(" UPDATE [{0}] SET {1} WHERE {2} ", this.GetTableName(), string.Join(",", listPropertyValueString.ToArray()), string.Join(" AND ", listWhereString.ToArray()));
        }

        public virtual string GetDeleteSQL()
        {
            var propertys = this.GetType().GetProperties();
            List<string> listPropertyValueString = new List<string>();

            if (string.IsNullOrWhiteSpace(this.GetIdentityKey()))
            {
                foreach (var filed in this.GetPrimaryKeys())
                {
                    listPropertyValueString.Add("[" + filed + "] = @" + filed);
                }               
            }
            else
            {
                listPropertyValueString.Add("[" + this.GetIdentityKey() + "] = @" + this.GetIdentityKey());
            }
            
        
            return string.Format(" DELETE FROM [{0}] WHERE {1} ", this.GetTableName(), string.Join(" AND ", listPropertyValueString.ToArray()));
        }

        /// <summary>
        /// 获取sql参数
        /// </summary>
        /// <returns></returns>
        public virtual SqlParameter[] GetParameters()
        {
            var propertys=this.GetType().GetProperties();
            
            List<SqlParameter> listSQLParamter=new List<SqlParameter>();
            foreach (var p in propertys)
            {
                listSQLParamter.Add(new SqlParameter() {ParameterName = p.Name, Value = p.GetValue(this,null) });
            }
            return listSQLParamter.ToArray();
        }

        /// <summary>
        /// 克隆对象
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        } 
    }
}
