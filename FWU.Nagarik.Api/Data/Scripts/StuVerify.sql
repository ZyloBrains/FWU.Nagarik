select * from grade

select * from ExamSubjectAndMarksRegistration

select * from DataAG

select sr.RegistrationNo, sr.BirthDateAD, sr.FirstName, sr.MiddleName, sr.LastName, ay.AcademicYearName IntakeYear, 'NA' as StudentStatus,
l.LevelName, c.CollegeName, 0.0 as CGPAScore, 'NA' as GraduateYear
from StudentRegistration sr
inner join AcademicYear ay on ay.AcademicYearID = sr.AcademicYearID
inner join Level l on sr.LevelID = l.LevelID
inner join College c on sr.CollegeID = c.CollegeID