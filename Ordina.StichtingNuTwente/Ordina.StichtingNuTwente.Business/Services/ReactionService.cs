using FastExcel;
using OfficeOpenXml;
using Ordina.StichtingNuTwente.Business.Helpers;
using Ordina.StichtingNuTwente.Business.Interfaces;
using Ordina.StichtingNuTwente.Data;
using Ordina.StichtingNuTwente.Models.Mappings;
using Ordina.StichtingNuTwente.Models.Models;
using Ordina.StichtingNuTwente.Models.ViewModels;
using System.Reflection;

namespace Ordina.StichtingNuTwente.Business.Services
{
    public class ReactionService : IReactionService
    {
        private readonly IRepository<Reactie> ReactieRepository;
        private readonly IRepository<Gastgezin> GastgezinRepository;
        private readonly IRepository<Persoon> PersoonRepository;
        private readonly IRepository<Adres> AdresRepository;
        private readonly IRepository<PlaatsingsInfo> PlaatsingsInfoRepository;
        private readonly IRepository<UserDetails> UserDetailsRepository;
        public ReactionService(IRepository<Reactie> reactieRepository, IRepository<Gastgezin> gastgezinRepository, IRepository<Persoon> persoonRepository, IRepository<Adres> adresRepository, IRepository<PlaatsingsInfo> plaatsingsInfoRepository, IRepository<UserDetails> userDetailsRepository)
        {
            ReactieRepository = reactieRepository;
            GastgezinRepository = gastgezinRepository;
            PersoonRepository = persoonRepository;
            AdresRepository = adresRepository;
            PlaatsingsInfoRepository = plaatsingsInfoRepository;
            UserDetailsRepository = userDetailsRepository;
        }

        public bool Save(AnswersViewModel viewModel, int? gastgezinId)
        {
            if (NewReactie(viewModel, gastgezinId) != null)
                return true;
            return false;
        }

        public Reactie NewReactie(AnswersViewModel viewModel, int? gastgezinId)
        {
            var dbmodel = ReactieMapping.FromWebToDatabaseModel(viewModel);
            dbmodel = ReactieRepository.Create(dbmodel);
            UpdateDatabaseWithRelationalObjects(viewModel, dbmodel, gastgezinId);
            if (dbmodel.Id > 0)
            {
                if (gastgezinId != null)
                {
                    var gastgezin = GastgezinRepository.GetById(gastgezinId.Value);
                    if (gastgezin != null)
                    {
                        if (gastgezin.Status == GastgezinStatus.Aangemeld)
                        {
                            var persoon = PersoonRepository.Get(x => x.Reactie != null && x.Reactie.Id == dbmodel.Id, "Reactie");
                            if (persoon != null)
                                gastgezin.Contact = persoon;

                            if (gastgezin.Comments == null)
                                gastgezin.Comments = new List<Comment>();
                            if (gastgezin.Intaker != null)
                                gastgezin.Comments.Add(new Comment("Intake uitgevoerd", gastgezin.Intaker, CommentType.INTAKE_COMPLETED));
                        }
                        gastgezin.IntakeFormulier = dbmodel;
                        GastgezinRepository.Update(gastgezin);
                    }
                }
                return dbmodel;
            }
            return null;
        }

        public void Update(AnswersViewModel viewModel, int id)
        {
            var dbmodel = ReactieMapping.FromWebToDatabaseModel(viewModel);
            dbmodel.Id = id;
            var existingReaction = ReactieRepository.GetById(id, "Antwoorden");
            foreach (var antwoord in dbmodel.Antwoorden)
            {
                var dbAntwoord = existingReaction.Antwoorden.SingleOrDefault(a => a.IdVanVraag == antwoord.IdVanVraag);
                if (dbAntwoord != null)
                {
                    dbAntwoord.Response = antwoord.Response;
                }
                else
                {
                    existingReaction.Antwoorden.Add(antwoord);
                }
            }

            ReactieRepository.Update(existingReaction);
            UpdateDatabaseWithRelationalObjects(viewModel, existingReaction, null, id);
        }

        public void UpdateAll(int? id = 0)
        {
            var reacties = ReactieRepository.GetAll("Antwoorden");
            if (id != 0 && id != null)
            {
                reacties = reacties.Where(r => r.FormulierId == id);
            }
            foreach (var reactie in reacties)
            {
                var viewModel = ReactieMapping.FromDatabaseToWebModel(reactie);
                try
                {
                    Update(viewModel, reactie.Id);
                }
                catch (Exception) { }
            }
        }

        public void UpdateDatabaseWithRelationalObjects(AnswersViewModel viewModel, Reactie reactie, int? gastgezinId, int id = 0)
        {
            int formId = 0;
            int.TryParse(viewModel.Id, out formId);
            var form = FormHelper.GetFormFromFileId(formId);
            var dbPersoon = new Persoon();
            var dbAdres = new Adres();
            var dbUser = new UserDetails();
            var dbPlaatsingsInfo = new PlaatsingsInfo();
            var dbGastgezin = GastgezinRepository.GetFirstOrDefault(g => (gastgezinId != null && g.Id == gastgezinId) || (g.IntakeFormulier != null && g.IntakeFormulier.Id == reactie.Id) || (g.IntakeFormulier != null && g.IntakeFormulier.Id == reactie.Id), "IntakeFormulier,AanmeldFormulier,PlaatsingsInfo,Buddy,Intaker");
            dbGastgezin = dbGastgezin ?? new Gastgezin();
            if (id != 0)
            {
                dbPersoon = PersoonRepository.GetFirstOrDefault(p => p.Reactie != null && p.Reactie.Id == id, "Reactie");
                dbPersoon = dbPersoon ?? new Persoon();
                dbAdres = AdresRepository.GetFirstOrDefault(p => p.Reactie != null && p.Reactie.Id == id, "Reactie");
                dbAdres = dbAdres ?? new Adres();
                dbPlaatsingsInfo = PlaatsingsInfoRepository.GetFirstOrDefault(p => p.Reactie != null && p.Reactie.Id == id, "Reactie") ?? new PlaatsingsInfo();
            }

            if (form.Sections.Any(s => s.Questions.Any(q => q.Object == "Adres")))
            {
                dbAdres = CreateDbObjectFromFormFilledWithAnswers<Adres>(form, viewModel, dbAdres);
                if (dbAdres != null)
                {
                    if (dbAdres.Id == 0)
                    {
                        dbAdres.Reactie = reactie;
                        AdresRepository.Create(dbAdres);
                    }
                    else
                    {
                        AdresRepository.Update(dbAdres);
                    }
                }
            }
            //attach user
            if (form.Sections.Any(s => s.Questions.Any(q => q.Object == "UserDetails")))
            {
                var questionId = form.Sections.FirstOrDefault(s => s.Questions.Any(q => q.Object == "UserDetails")).Questions.FirstOrDefault(q => q.Object == "UserDetails").Id;
                if (viewModel.answer.Any())
                {
                    var awnser = viewModel.answer.FirstOrDefault(a => a.Nummer.Trim() == questionId.ToString());
                    if (awnser != null)
                    {
                        var userNameAndEmail = awnser.Antwoord;
                        if (userNameAndEmail.Contains("(") && userNameAndEmail.Contains(")"))
                        {
                            var email = userNameAndEmail.Split("(")[1].Split(")")[0];
                            dbUser = UserDetailsRepository.GetFirstOrDefault(u => u.Email.Contains(email) && u.InDropdown == true);
                            if (dbUser != null)
                            {
                                if (dbUser.Reacties != null)
                                {
                                    dbUser.Reacties.Add(reactie);
                                }
                                else
                                {
                                    dbUser.Reacties = new List<Reactie>() { reactie };
                                }
                                UserDetailsRepository.Update(dbUser);
                            }
                        }
                        if ( form.Id == 2 && dbGastgezin != new Gastgezin())
                        {
                            if (dbGastgezin.Intaker == null)
                            {
                                dbGastgezin.Intaker = reactie.UserDetails;
                                GastgezinRepository.Update(dbGastgezin);
                            }

                            if (dbGastgezin.Buddy == null)
                            {
                                dbGastgezin.Buddy = dbGastgezin.Intaker;
                                GastgezinRepository.Update(dbGastgezin);
                            }
                        }
                    }
                }
            }
            if (form.Sections.Any(s => s.Questions.Any(q => q.Object == "Persoon")))
            {
                dbPersoon = CreateDbObjectFromFormFilledWithAnswers<Persoon>(form, viewModel, dbPersoon);
                if (dbPersoon != null)
                {
                    if (dbPersoon.Id == 0)
                    {
                        if (dbAdres != null)
                        {
                            dbPersoon.Adres = dbAdres;
                        }
                        dbPersoon.Reactie = reactie;
                        PersoonRepository.Create(dbPersoon);
                    }
                    else
                    {
                        PersoonRepository.Update(dbPersoon);
                        if (form.Id == 2)
                        {
                            var gastgezin = GastgezinRepository.GetFirstOrDefault(g => g.IntakeFormulier != null && g.IntakeFormulier.Id == reactie.Id, "IntakeFormulier");
                            if (gastgezin != null)
                            {
                                gastgezin.Contact = dbPersoon;
                                GastgezinRepository.Update(gastgezin);
                            }
                        }
                    }
                }
            }
            //Formulier: Gastgezin aanmelden
            if (form.Id == 1 && id == 0)
            {
                var gastgezin = new Gastgezin
                {
                    AanmeldFormulier = reactie,
                    Contact = dbPersoon
                };

                dbGastgezin = GastgezinRepository.Create(gastgezin);
            }

            if (form.Sections.Any(s => s.Questions.Any(q => q.Object == "PlaatsingsInfo")))
            {
                dbPlaatsingsInfo = CreateDbObjectFromFormFilledWithAnswers<PlaatsingsInfo>(form, viewModel, dbPlaatsingsInfo);
                if (dbPlaatsingsInfo != null)
                {
                    if (dbPlaatsingsInfo.Id == 0)
                    {
                        if (dbGastgezin != null && dbGastgezin.Id > 0)
                        {
                            dbPlaatsingsInfo.Reactie = reactie;
                            dbPlaatsingsInfo = PlaatsingsInfoRepository.Create(dbPlaatsingsInfo);
                            dbGastgezin = GastgezinRepository.GetById(dbGastgezin.Id);
                            if (dbPlaatsingsInfo.Id != 0)
                            {
                                dbGastgezin.PlaatsingsInfo = dbPlaatsingsInfo;
                                GastgezinRepository.Update(dbGastgezin);
                            }
                        }
                    }
                    else
                    {
                        PlaatsingsInfoRepository.Update(dbPlaatsingsInfo);
                        if (dbGastgezin != null && dbGastgezin.PlaatsingsInfo == null)
                        {
                            dbGastgezin.PlaatsingsInfo = dbPlaatsingsInfo;
                            GastgezinRepository.Update(dbGastgezin);
                        }
                    }
                }
            }
        }

        private T CreateDbObjectFromFormFilledWithAnswers<T>(Form form, AnswersViewModel viewModel, T classObject)
        {
            var typeObject = typeof(T);
            foreach (var prop in typeObject.GetProperties())
            {
                foreach (var s in form.Sections)
                {
                    foreach (var q in s.Questions)
                    {
                        var answer = viewModel.answer.FirstOrDefault(a => a.Nummer.Trim() == q.Id.ToString());
                        if (answer != null && q.ParameterName == prop.Name && q.Object == typeObject.Name)
                        {
                            PropertyInfo propInfo = typeObject.GetProperty(prop.Name);
                            if (propInfo != null)
                            {
                                object value = null;
                                switch (prop.PropertyType.Name.ToLower())
                                {
                                    case "string":
                                        value = answer.Antwoord;
                                        break;

                                    case "boolean":
                                        value = Convert.ToBoolean(answer.Antwoord);
                                        break;

                                    case "datetime":
                                        value = Convert.ToDateTime(answer.Antwoord);
                                        break;

                                    case "int32":
                                        value = Convert.ToInt32(answer.Antwoord);
                                        break;

                                    case "decimal":
                                        value = Convert.ToDecimal(answer.Antwoord);
                                        break;

                                    default:
                                        value = answer.Antwoord;
                                        break;
                                }
                                propInfo.SetValue(classObject, value);
                            }
                        }
                    }
                }
            }
            return classObject;
        }

        public Form GetAnwersFromId(int Id)
        {
            var viewModel = new Form();
            var dbModel = ReactieRepository.GetById(Id, "Antwoorden,Comments");
            if (dbModel != null)
            {
                viewModel = FormHelper.GetFormFromFileId(dbModel.FormulierId);
                foreach (var section in viewModel.Sections)
                {
                    foreach (var question in section.Questions)
                    {
                        var antwoord = dbModel.Antwoorden.FirstOrDefault(a => a.IdVanVraag == question.Id);
                        if (antwoord != null)
                        {
                            question.Answer = antwoord.Response;
                        }
                    }
                }

                viewModel.Deleted = dbModel.Deleted;
                Comment? deletionComment = null;

                if (dbModel.Comments != null && dbModel.Comments.Count > 0)
                {
                    deletionComment = dbModel.Comments.LastOrDefault(g => g.CommentType == CommentType.DELETION);
                }
                viewModel.DeletionComment = deletionComment;
            }
            return viewModel;
        }

        public Reactie GetReactieFromId(int Id)
        {
            return ReactieRepository.GetById(Id, "UserDetails,Comments");
        }

        public List<AnswerListModel> GetAllRespones(int? form = null)
        {
            List<AnswerListModel> viewModel = new List<AnswerListModel>();

            var reacties = ReactieRepository.GetAll();
            var people = PersoonRepository.GetAll("Reactie,Adres");
            reacties = reacties.Where(r => !r.Deleted);
            if (form != null)
            {
                reacties = reacties.Where(f => f.FormulierId == form.Value);
            }
            var t = from reactie in reacties
                    join add in people
                    on reactie.Id equals add.Reactie?.Id
                    into PeopleReaction
                    from persoon in PeopleReaction.DefaultIfEmpty()
                    select new { reactie, persoon };

            viewModel = t.OrderByDescending(x => x.reactie.Id).ToList().ConvertAll(p =>
            {
                var awnser = ReactieMapping.FromDatabaseToWebListModel(p.reactie);
                awnser.Persoon = p.persoon;
                return awnser;
            });
            return viewModel;
        }

        public bool Delete(int reactionId, string comment, UserDetails user)
        {
            var existingReaction = ReactieRepository.GetById(reactionId, "Antwoorden,Comments");
            existingReaction.Deleted = true;
            if (existingReaction.Comments == null)
                existingReaction.Comments = new List<Comment>();
            existingReaction.Comments.Add(new Comment(comment, user, CommentType.DELETION));

            ReactieRepository.Update(existingReaction);
            return true;
        }

        public bool Restore(int reactionId)
        {

            var existingReaction = ReactieRepository.GetById(reactionId, "Antwoorden,Comments");
            existingReaction.Deleted = false;
            ReactieRepository.Update(existingReaction);
            return true;
        }

        public IEnumerable<Reactie> GetDeletedReacties()
        {
            var reacties = ReactieRepository.GetAll("Comments").Where(r => r.Deleted);
            return reacties;
        }
    }
}