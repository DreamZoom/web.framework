using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dz.web.model;
using dz.web.Html;
namespace dz.web.service
{
    public class ServiceBase : IService
    {
        #region 基本操作 增/删/改/查
        public virtual bool Insert(ModelBase model)
        {
            return DBUtility.DbHelperSQL.ExecuteSql(model.GetInsertSQL(), model.GetParameters()) > 0;
        }

        public virtual bool Update(ModelBase model)
        {
            return DBUtility.DbHelperSQL.ExecuteSql(model.GetUpdateSQL(), model.GetParameters()) > 0;
        }

        public virtual bool Delete(ModelBase model)
        {
            if (model == null) return false;
            return DBUtility.DbHelperSQL.ExecuteSql(model.GetDeleteSQL(), model.GetParameters()) > 0;
        }

        public virtual ModelBase GetModel(string where)        
        {
            var model = this.CreateTemplate();

            if (model == null) return null;

            string sql = model.GetSelectSQL(1);

            if (!string.IsNullOrWhiteSpace(where))
            {
                sql +=" where " + where;
            }

            var dt = DBUtility.DbHelperSQL.Query(sql);

            if (dt.Tables.Count > 0)
            {
               return TableToModelList(dt.Tables[0], model).FirstOrDefault();
            }
            return null;
        }

        #endregion

        #region 通用方法

        public virtual string GetServiceName()
        {
            return this.GetType().Name.Replace("Service", "");
        }

       
        public virtual ModelBase CreateTemplate()
        {
            foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {

                if (assembly.GetName().Name == "Anonymously Hosted DynamicMethods Assembly") continue;

                Type serviceType = assembly.GetTypes().FirstOrDefault(m => m.Name == (this.GetServiceName()));
                if (serviceType != null)
                {
                    return assembly.CreateInstance(serviceType.FullName) as ModelBase;
                }

            }
            return null;
        }

        public virtual ModelBase RowToModel(System.Data.DataRow dr, ModelBase template)
        {
            var propertys = template.GetType().GetProperties();
            foreach (var p in propertys)
            {
                if (dr.Table.Columns.Contains(p.Name) && dr[p.Name] != null)
                {
                    p.SetValue(template, dr[p.Name], null);
                }
            }
            return template.Clone() as ModelBase;
        }
        public virtual List<ModelBase> TableToModelList(System.Data.DataTable table, ModelBase template)
        {
            List<ModelBase> list = new List<ModelBase>();
            foreach (System.Data.DataRow dr in table.Rows)
            {
                list.Add(RowToModel(dr, template));
            }
            return list;
        }
        #endregion

        #region 列表查询和分页查询

        public virtual IEnumerable<ModelBase> GetModelList(string where, string order, int top)
        {
            List<ModelBase> list = new List<ModelBase>();
            
            var model = this.CreateTemplate();
            if (model == null) return list;

            string sql =model.GetSelectSQL();

            if(!string.IsNullOrWhiteSpace(where)) sql+=" where "+where;

            if (!string.IsNullOrWhiteSpace(order)) sql += " order by " + order;

            sql= DBUtility.DbHelperSQL.CreateTopnSql(top, sql);

            var dt = DBUtility.DbHelperSQL.Query(sql);

            if (dt.Tables.Count > 0)
            {
                return TableToModelList(dt.Tables[0], model);
            }

            return list;
        }

        public virtual TableListed GetModelList(string where, string order, int page, int pagesize)
        {
            TableListed tList = new TableListed();


            var model = this.CreateTemplate();
            if (model == null) return tList;

            string sql = model.GetSelectSQL();

            if (!string.IsNullOrWhiteSpace(where)) sql += " where " + where;

            if (string.IsNullOrWhiteSpace(order)) order = string.IsNullOrWhiteSpace(model.GetIdentityKey()) ? string.Join(",",model.GetPrimaryKeys()) : model.GetIdentityKey(); ;//如果排序字段为空，则默认

            int recordCount = (int)DBUtility.DbHelperSQL.GetSingle(sql);
            

            sql = DBUtility.DbHelperSQL.CreatePagingSql(recordCount, pagesize, page, sql, order);

            var dt = DBUtility.DbHelperSQL.Query(sql);

            if (dt.Tables.Count > 0)
            {
                tList.AddRange(TableToModelList(dt.Tables[0], model));
            }
            
            tList.RecordCount = recordCount;
            tList.PageSize = pagesize;
            tList.PageIndex = page;

            tList.ActionLinks = this.ActionLinks;
            tList.BatchActionLinks = this.BatchActionLinks;

            tList.DataType = model.GetType();

            return tList;
        }
        #endregion


        #region 操作相关
        /// <summary>
        /// 列表操作链接
        /// </summary>
        protected List<KeyValuePair<string,string>> ActionLinks { get; set; }

        /// <summary>
        /// 批量操作链接
        /// </summary>
        protected List<KeyValuePair<string, string>> BatchActionLinks { get; set; }

        /// <summary>
        /// 添加操作链接
        /// </summary>
        /// <param name="actionLink"></param>
        public void AddActionLinks(KeyValuePair<string, string> actionLink)
        {
            if (ActionLinks == null) ActionLinks = new List<KeyValuePair<string, string>>();

            if (ActionLinks.Contains(actionLink)) throw new Exception("已有相同链接");

            ActionLinks.Add(actionLink);
        }

        /// <summary>
        /// 添加批量操作链接
        /// </summary>
        /// <param name="actionLink"></param>
        public void AddBatchActionLinks(KeyValuePair<string, string> actionLink)
        {
            if (BatchActionLinks == null) BatchActionLinks = new List<KeyValuePair<string, string>>();

            if (BatchActionLinks.Contains(actionLink)) throw new Exception("已有相同链接");

            BatchActionLinks.Add(actionLink);
        }

        #endregion
    }
}
