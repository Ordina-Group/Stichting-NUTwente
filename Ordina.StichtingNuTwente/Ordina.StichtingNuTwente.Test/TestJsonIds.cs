using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Ordina.StichtingNuTwente.Models.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Ordina.StichtingNuTwente.Test
{
    [TestClass]
    public class TestJsonIds
    {
        [TestMethod]
        public void GastgezinAanmelding_UniqueQuestionIds()
        {
            var fileName = "GastgezinAanmelding.json";
            var form = GetForm(fileName);
            Assert.IsTrue(UniqueIds(form), "Duplicate Ids in form: " + form.Title);
        }

        [TestMethod]
        public void GastgezinIntake_UniqueQuestionIds()
        {
            var fileName = "GastgezinIntake.json";
            var form = GetForm(fileName);
            Assert.IsTrue(UniqueIds(form), "Duplicate Ids in form: " + form.Title);
        }

        [TestMethod]
        public void VrijwilligerAanmelding_UniqueQuestionIds()
        {
            var fileName = "VrijwilligerAanmelding.json";
            var form = GetForm(fileName);
            Assert.IsTrue(UniqueIds(form), "Duplicate Ids in form: " + form.Title);
        }

        [TestMethod]
        public void VluchtelingIntake_UniqueQuestionIds()
        {
            var fileName = "VluchtelingIntake.json";
            var form = GetForm(fileName);
            Assert.IsTrue(UniqueIds(form), "Duplicate Ids in form: " + form.Title);
        }

        private static Form GetForm(string fileName)
        {
            string jsonString = Encoding.UTF8.GetString(File.ReadAllBytes(fileName));
            var form = JObject.Parse(jsonString).ToObject<Form>();
            return form;
        }

        private static bool UniqueIds(Form form)
        {
            var ids = new List<int>();
            foreach (Section section in form.Sections)
            {
                foreach (Question question in section.Questions)
                {
                    ids.Add(question.Id);
                }
            }
            return ids.Count == ids.Distinct().Count();
        }
    }
}
