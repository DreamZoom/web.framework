using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dz.web.Html
{
    public class TableListed : List<object>, ITableListed
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                return RecordCount / PageSize + (RecordCount % PageSize == 0 ? 0 : 1);
            }
        }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int RecordCount { get; set; }



        /// <summary>
        /// 数据类型
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// 操作链接
        /// </summary>
        public List<KeyValuePair<string, string>> ActionLinks { get; set; }


        /// <summary>
        /// 批量操作
        /// </summary>
        public List<KeyValuePair<string, string>> BatchActionLinks { get; set; }

    }


    
}
