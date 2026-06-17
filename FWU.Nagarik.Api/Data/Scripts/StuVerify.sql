select * from grade

select StudentAdmissionID, * from ExamRegistration

select * from VwExamRegistration

select * from ExamSubjectAndMarksRegistration

select * from StudentRegistration

select * from StudentAdmission

select * from StudentQualification

select * from GraceListDetail

select * from PrintedDocument where DocumentTypeName='Transcript' and EndYear='2024'


select sr.RegistrationNo RegdNo, sr.BirthDateAD DobAD, sr.FirstName, sr.MiddleName, sr.LastName, p.ProgramName, ay.AcademicYearName IntakeYear, 'NA' as StudentStatus,
l.LevelName Level, c.CollegeName School, sa.CGPA as CGPAScore, sa.EndYear as GraduateYear
--INTO #TempStuVerify
from StudentAdmission sa
left join StudentRegistration sr on sa.StudentRegistrationId = sr.StudentRegistrationId
left join AcademicYear ay on ay.AcademicYearID = sa.AcademicYearID
left join Level l on sr.LevelID = l.LevelID
left join College c on sa.CollegeID = c.CollegeID
left join Program p on p.ProgramID = sa.ProgramID
--left join PrintedDocument pd on pd.StudentAdmissionID = sa.StudentAdmissionID
--where pd.DocumentTypeName='Transcript'

--select * from #TempStuVerify
--where IntakeYear IS NOT NULL



