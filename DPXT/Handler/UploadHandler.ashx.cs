using Comp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DPXT.Handler
{
    /// <summary>
    /// UploadHandler 的摘要说明
    /// </summary>
    public class UploadHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
 
      
            context.Response.Charset = "utf-8";
            HttpPostedFile file = context.Request.Files["files"];
            string ext = Path.GetExtension(file.FileName);
            string uploadPath =
                HttpContext.Current.Server.MapPath( "\\upimg\\");
            string filename = Guid.NewGuid().ToString("N") + ext;
            if (file != null)
            {
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                
                file.SaveAs(uploadPath + filename);
                //下面这句代码缺少的话，上传成功后上传队列的显示不会自动消失
                context.Response.Write(filename);
            }
            else
            {
                context.Response.Write("0");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}