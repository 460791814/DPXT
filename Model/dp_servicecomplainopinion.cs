using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
   public class dp_servicecomplainopinion
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
        /// 意见id
        /// </summary>		
        private int _opinionid;
        public int opinionid
        {
            get { return _opinionid; }
            set { _opinionid = value; }
        }

    }
}
