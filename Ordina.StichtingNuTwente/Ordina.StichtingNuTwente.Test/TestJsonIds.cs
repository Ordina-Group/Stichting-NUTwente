using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ordina.StichtingNuTwente.Models.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            var duplicateIds = DuplicateIds(form);
            Assert.IsTrue(duplicateIds.Count == 0, $"Duplicate Ids {string.Join(",", duplicateIds)} in form {form.Title}");
        }

        [TestMethod]
        public void GastgezinIntake_UniqueQuestionIds()
        {
            var fileName = "GastgezinIntake.json";
            var form = GetForm(fileName);
            var duplicateIds = DuplicateIds(form);
            Assert.IsTrue(duplicateIds.Count == 0, $"Duplicate Ids {string.Join(",", duplicateIds)} in form {form.Title}");
        }

        [TestMethod]
        public void VrijwilligerAanmelding_UniqueQuestionIds()
        {
            var fileName = "VrijwilligerAanmelding.json";
            var form = GetForm(fileName);
            var duplicateIds = DuplicateIds(form);
            Assert.IsTrue(duplicateIds.Count == 0, $"Duplicate Ids {string.Join(",", duplicateIds)} in form {form.Title}");
        }

        [TestMethod]
        public void VluchtelingIntake_UniqueQuestionIds()
        {
            var fileName = "VluchtelingIntake.json";
            var form = GetForm(fileName);
            var duplicateIds = DuplicateIds(form);
            Assert.IsTrue(duplicateIds.Count == 0, $"Duplicate Ids {string.Join(",", duplicateIds)} in form {form.Title}");
        }

        private static Form GetForm(string fileName)
        {
            string jsonString = File.ReadAllText(fileName, Encoding.UTF8);
            Form form = JsonSerializer.Deserialize<Form>(jsonString);
            return form;
        }

        private static List<int> DuplicateIds(Form form)
        {
            var ids = new List<int>();
            foreach (Section section in form.Sections)
            {
                foreach (Question question in section.Questions)
                {
                    ids.Add(question.Id);
                }
            }
            var duplicates = ids.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            return duplicates;
        }
    }
}
