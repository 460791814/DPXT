using DAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Timers;

namespace DPXT.Mousse
{
    /// <summary>
    /// 菜同步
    /// </summary>
    public class FoodSync
    {
        D_Catering dal = new D_Catering();
        D_Food dFood = new D_Food();
        Thread thread = null;
        System.Timers.Timer myTimer;
        public void Start()
        {
            myTimer = new System.Timers.Timer(1000*5);//定时周期2秒
            myTimer.Elapsed += myTimer_Elapsed;//到2秒了做的事件
            myTimer.AutoReset = true; //是否不断重复定时器操作
            myTimer.Enabled = true;
         
        }

        private void myTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            thread = new Thread(Sync);
            thread.IsBackground = true;
            thread.Start();
        }

        public void Sync() {
            DateTime currTime = Convert.ToDateTime(DateTime.Now.AddMonths(-8).ToString("D"));
          List<E_Dish> list= dal.GetDishCurrDay(currTime, 1, 42);
 
           

            if (list != null)
            {
                foreach (var item in list)
                {
                  E_Food eFood=  dFood.GetInfoByDishId(new E_Food() { dishid = item.id, addtime = currTime });
                    if (eFood == null)
                    {
                        dFood.Add(new E_Food()
                        {
                            areaid = 1,
                            classinfoid = 42,
                            dishid = item.id,
                            foodname = item.Name,
                            pic = item.Picture
                        });
                    }
                    else {
                        dFood.Update(new E_Food()
                        {
                            
                           foodid=eFood.foodid,
                            foodname = item.Name,
                            pic = item.Picture
                        });
                    }
                }
            }
          

        }
    }
}