using CaptivePortal.API.Context;
using CaptivePortal.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CaptivePortal.API.Repository
{
    public class FormControlRepository
    {
        private int RetCode { get; set; }
        private string RetMsg { get; set; }
        private DbContext db = null;
        StringBuilder sb = new StringBuilder(String.Empty);

        public FormControlRepository()
        {
            db = new DbContext();
        }
        public bool CreateFormControl(FormControlViewModel model, string IsMandetory)
        {
            try
            {
                FormControl objFormControl = new FormControl();
                objFormControl.ControlType = model.controlType;
                objFormControl.LabelName = model.fieldlabel;
                objFormControl.LabelNameToDisplay = model.LabelNameToDisplay;
                objFormControl.IsMandetory = Convert.ToBoolean(IsMandetory);
                objFormControl.FormId = model.FormId;
                objFormControl.HtmlString = sb.ToString();
                db.FormControl.Add(objFormControl);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
    }
}