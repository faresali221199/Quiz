using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quiz.Data;
using Quiz.Models;
using System.Text.Json;

namespace Quiz.Controllers
{
    public class AdminController : Controller
    {
        private readonly QuizDBDbContext _context;

        public AdminController(QuizDBDbContext context)
        {
            _context = context;
        }

        // 1. لوحة التحكم (Dashboard)
        public async Task<IActionResult> Dashboard()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");

            try
            {
                ViewBag.SubjectsCount = await _context.Subjects.AsNoTracking().CountAsync();
                ViewBag.QuestionsCount = await _context.Questions.AsNoTracking().CountAsync();
                ViewBag.LecturesCount = await _context.Lectures.AsNoTracking().CountAsync();
                ViewBag.StudentsCount = await _context.Users.AsNoTracking().CountAsync();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "حدث خطأ أثناء تحميل البيانات: " + ex.Message;
            }

            return View("~/Views/Admin/Dashboard.cshtml");
        }

        // 2. عرض نتائج الطلاب (StudentsScores) - الكود اللي بعته يا دكتور
        public async Task<IActionResult> StudentsScores()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");

            var results = await _context.Results
                .Include("User")
                .Include("Lecture")
                .AsNoTracking()
                .ToListAsync();

            return View("~/Views/Admin/StudentsScores.cshtml", results);
        }

        // 3. حذف نتيجة اختبار
        [HttpPost]
        public async Task<IActionResult> DeleteResult(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return Unauthorized();

            var result = await _context.Results.FindAsync(id);
            if (result != null)
            {
                _context.Results.Remove(result);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(StudentsScores));
        }

        // 4. عرض المواد والمحاضرات (ViewData)
        public async Task<IActionResult> ViewData()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true") return RedirectToAction("Login");

            var subjects = await _context.Subjects
                .Include("Lectures")
                .Include("Year")
                .AsNoTracking()
                .OrderByDescending(s => s.SubjectId)
                .ToListAsync();

            return View("~/Views/Admin/ViewData.cshtml", subjects);
        }

        // 5. رفع ملف الأسئلة (JSON)
        [HttpGet]
        public IActionResult Upload() => View("~/Views/Admin/Upload.cshtml");

        [HttpPost]
        public async Task<IActionResult> Upload(int yearId, int term, IFormFile jsonFile)
        {
            if (jsonFile == null || jsonFile.Length == 0) return View();

            try
            {
                using var reader = new StreamReader(jsonFile.OpenReadStream());
                var content = await reader.ReadToEndAsync();
                var jsonData = JsonSerializer.Deserialize<AdminJsonData>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (jsonData == null) return View();

                var subject = await _context.Subjects.FirstOrDefaultAsync(s =>
                    s.SubjectName == jsonData.Subject && s.YearId == yearId && s.Term == term);

                if (subject == null)
                {
                    subject = new Subject { SubjectName = jsonData.Subject, YearId = yearId, Term = term };
                    _context.Subjects.Add(subject);
                    await _context.SaveChangesAsync();
                }

                var lecture = await _context.Lectures.FirstOrDefaultAsync(l =>
                    l.SubjectId == subject.SubjectId && l.LectureTitle.Contains(jsonData.LectureNumber.ToString()));

                if (lecture == null)
                {
                    lecture = new Lecture { SubjectId = subject.SubjectId, LectureTitle = $"المحاضرة رقم {jsonData.LectureNumber}" };
                    _context.Lectures.Add(lecture);
                    await _context.SaveChangesAsync();
                }

                foreach (var q in jsonData.Questions)
                {
                    var question = new Question { LectureId = lecture.LectureId, QuestionText = q.Text };
                    _context.Questions.Add(question);
                    await _context.SaveChangesAsync();

                    foreach (var opt in q.Options)
                    {
                        _context.Answers.Add(new Answer
                        {
                            QuestionId = question.QuestionId,
                            AnswerText = opt.Value,
                            IsCorrect = (opt.Key == q.CorrectAnswer)
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }
            catch
            {
                ViewBag.Error = "حدث خطأ في قراءة ملف JSON. تأكد من الصيغة الصحيحة.";
                return View();
            }
        }

        // 6. تسجيل الدخول والخروج للآدمن
        [HttpGet]
        public IActionResult Login() => View("~/Views/Admin/Login.cshtml");

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "admin123")
            {
                HttpContext.Session.SetString("IsAdmin", "true");
                return RedirectToAction(nameof(Dashboard));
            }
            ViewBag.Error = "بيانات الدخول غير صحيحة";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        // كلاسات مساعدة للـ JSON
        public class AdminJsonData
        {
            public string Subject { get; set; } = null!;
            public int LectureNumber { get; set; }
            public List<AdminJsonQuestion> Questions { get; set; } = new();
        }

        public class AdminJsonQuestion
        {
            public string Text { get; set; } = null!;
            public Dictionary<string, string> Options { get; set; } = new();
            public string CorrectAnswer { get; set; } = null!;
        }
    }
}