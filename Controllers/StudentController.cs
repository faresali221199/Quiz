using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quiz.Data;
using Quiz.Models;
using System.Text.Json;

namespace Quiz.Controllers
{
    public class StudentController : Controller
    {
        private readonly QuizDBDbContext _context;
        public StudentController(QuizDBDbContext context) { _context = context; }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartSession(string fullName, string academicId, int section, int yearId, string phone, int term)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.AcademicID == academicId);
            if (user == null)
            {
                user = new User { FullName = fullName, AcademicID = academicId, Section = section, YearId = yearId, Phone = phone, Term = term, CreatedAt = DateTime.Now };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            HttpContext.Session.SetInt32("StudentId", user.UserId);
            HttpContext.Session.SetInt32("YearId", yearId);
            HttpContext.Session.SetInt32("Term", term);
            return RedirectToAction("SelectSubject");
        }

        public async Task<IActionResult> SelectSubject()
        {
            int? year = HttpContext.Session.GetInt32("YearId");
            int? term = HttpContext.Session.GetInt32("Term");
            if (year == null) return RedirectToAction("Login");
            var subjects = await _context.Subjects.Where(s => s.YearId == year && s.Term == term).AsNoTracking().ToListAsync();
            return View(subjects);
        }

        public async Task<IActionResult> SelectLecture(int subjectId)
        {
            var lectures = await _context.Lectures.Where(l => l.SubjectId == subjectId).AsNoTracking().ToListAsync();
            var subject = await _context.Subjects.FindAsync(subjectId);
            ViewBag.SubjectName = subject?.SubjectName;
            ViewBag.SubjectId = subjectId;
            return View(lectures);
        }

        // ميثود بدء الاختبار: الموديل هنا List<Question>
        public async Task<IActionResult> StartQuiz(int lectureId)
        {
            var questions = await _context.Questions.Include(q => q.Answers)
                .Where(q => q.LectureId == lectureId).OrderBy(q => Guid.NewGuid()).Take(10).AsNoTracking().ToListAsync();

            if (!questions.Any()) return RedirectToAction("SelectSubject");

            ViewBag.LectureId = lectureId;
            HttpContext.Session.SetString("QuizStart", DateTime.Now.ToString("o"));
            return View(questions);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitQuiz(Dictionary<int, int> answers, int lectureId)
        {
            int studentId = HttpContext.Session.GetInt32("StudentId") ?? 0;
            if (studentId == 0 || lectureId == 0) return RedirectToAction("Login");

            int score = 0;
            var wrongDetails = new List<object>();

            foreach (var entry in answers)
            {
                var q = await _context.Questions.Include(x => x.Answers).FirstOrDefaultAsync(x => x.QuestionId == entry.Key);
                var correctAns = q.Answers.FirstOrDefault(a => a.IsCorrect);
                var studentAns = q.Answers.FirstOrDefault(a => a.AnswerId == entry.Value);

                if (studentAns != null && studentAns.IsCorrect) score++;
                else
                {
                    wrongDetails.Add(new { Question = q.QuestionText, Correct = correctAns?.AnswerText, Yours = studentAns?.AnswerText ?? "لم تجب" });
                }
            }

            var startTimeStr = HttpContext.Session.GetString("QuizStart");
            DateTime quizStart = string.IsNullOrEmpty(startTimeStr) ? DateTime.Now : DateTime.Parse(startTimeStr);

            var result = new Result { UserId = studentId, LectureId = lectureId, TotalScore = score, StartTime = quizStart, EndTime = DateTime.Now };
            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            TempData["WrongOnes"] = JsonSerializer.Serialize(wrongDetails);
            return RedirectToAction("ShowResult", new { id = result.ResultId });
        }

        // ميثود عرض النتيجة: الموديل هنا Result
        public async Task<IActionResult> ShowResult(int id)
        {
            var res = await _context.Results.Include(r => r.User).Include(r => r.Lecture).FirstOrDefaultAsync(x => x.ResultId == id);
            if (res == null) return RedirectToAction("SelectSubject");

            if (res.StartTime.HasValue && res.EndTime.HasValue)
            {
                TimeSpan duration = res.EndTime.Value - res.StartTime.Value;
                ViewBag.DurationFormatted = $"{(int)duration.TotalMinutes} دقيقة و {duration.Seconds} ثانية";
            }
            else { ViewBag.DurationFormatted = "غير متوفر"; }

            ViewBag.Percentage = (res.TotalScore / 10.0) * 100;
            var wrongJson = TempData["WrongOnes"] as string;
            ViewBag.WrongDetails = string.IsNullOrEmpty(wrongJson) ? null : JsonSerializer.Deserialize<List<JsonElement>>(wrongJson);
            return View(res);
        }
    }
}