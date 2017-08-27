using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
   public class dp_servicecomplain
    {
        /// <summary>
        /// 投诉记录id
        /// </summary>		
        private int _servicecomplainid;
        public int servicecomplainid
        {
            get { return _servicecomplainid; }
            set { _servicecomplainid = value; }
        }
        /// <summary>
        /// 作业区ID
        /// </summary>		
        private int _areaid;
        public int areaid
        {
            get { return _areaid; }
            set { _areaid = value; }
        }
        /// <summary>
        /// 班组id
        /// </summary>		
        private int _classinfoid;
        public int classinfoid
        {
            get { return _classinfoid; }
            set { _classinfoid = value; }
        }
        /// <summary>
        /// 投诉内容
        /// </summary>		
        private string _contents;
        public string contents
        {
            get { return _contents; }
            set { _contents = value; }
        }
        /// <summary>
        /// 投诉时间
        /// </summary>		
        private DateTime _addtime;
        public DateTime addtime
        {
            get { return _addtime; }
            set { _addtime = value; }
        }

        public List<dp_servicecomplainopinion> opinionlist { get; set; }
    }
}
