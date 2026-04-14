# Business Proposal
## Far Western University - Student Verification & Transcript API System

---

**Prepared By:** Blend Technologies Pvt. Ltd.  
**Date:** April 14, 2026  
**Version:** 1.0

---

## Table of Contents

1. Executive Summary
2. Background & Problem Statement
3. Proposed Solution
4. Key Benefits
5. Scope of Work
6. Technical Architecture
7. Implementation Timeline
8. Cost Breakdown
9. Terms & Conditions
10. Contact Information

---

## 1. Executive Summary

Far Western University (FWU) requires a modern, reliable API system to verify student enrollment details and provide academic transcript data to external consumers. This proposal outlines the development of a robust ASP.NET Core 10 Minimal API solution that will enable authorized agencies to verify student credentials and generate official transcripts digitally.

The proposed solution will:
- Provide real-time student verification using registration number and date of birth
- Return comprehensive academic details including program, enrollment status, and CGPA
- Supply detailed marks/grades for transcript generation
- Maintain audit logs for all verification requests

**Investment Overview:**
- One-Time Development Cost: NPR 211,000
- Annual Recurring Cost: NPR 55,000
- 1-Year Support Package: NPR 75,000 (included)
- **Total First-Year Investment: NPR 341,000**

---

## 2. Background & Problem Statement

### Current Challenges

Far Western University currently faces challenges in providing automated student verification services to external agencies and consumers:

| Challenge | Impact |
|-----------|--------|
| Manual Verification Process | Time-consuming, prone to errors, requires dedicated staff |
| No Digital Transcript API | External agencies cannot programmatically generate transcripts |
| Lack of Audit Trail | No systematic record of who accessed student information |
| Limited Integration Options | Other government systems cannot integrate for verification |
| Delayed Response Times | Manual process takes days instead of seconds |

### Opportunity

With the Government of Nepal mandating online verification systems through the Nagarik App initiative, FWU needs to:
- Enable instant verification of student enrollment
- Provide programmatic access to academic records
- Compress with national digital transformation standards
- Reduce administrative burden on university staff

---

## 3. Proposed Solution

### 3.1 Solution Overview

We propose developing a RESTful API system using ASP.NET Core 10 Minimal API architecture with Entity Framework Core and SQL Server database.

### 3.2 Core Features

#### Student Verification API
- **Endpoint:** `GET /api/student/verify`
- **Parameters:** `registration_number`, `dobAD`
- **Returns:** Student enrollment details, program info, enrollment status, CGPA
- **Use Case:** External agencies verify student credentials for employment, further education, visa applications

#### Transcript API
- **Endpoint:** `GET /api/student/transcript`
- **Parameters:** `registration_number`, `dobAD`
- **Returns:** Comprehensive marks/grades across all subjects, semesters, academic years
- **Use Case:** Generate official transcripts for students applying to foreign universities or employers

#### Audit Logging
- Automatic logging of all API requests
- Records: timestamp, registration number, request outcome, IP address
- Supports compliance and accountability requirements

### 3.3 API Response Format

```json
{
  "data": "027819-20",
  "message": "Success",
  "otherData": [{
    "regdNo": "027819-20",
    "firstName": "SAUGAT",
    "middleName": "",
    "lastName": "GURUNG",
    "dobAD": "2001-11-25",
    "programName": "Bachelor of Engineering in Mechanical Engineering",
    "intakeYear": "December 2020",
    "studentStatus": "Running",
    "level": "Bachelor",
    "school": "School of Engineering",
    "cgpaScore": null,
    "graduateYear": null
  }]
}
```

---

## 4. Key Benefits

### 4.1 Immediate Benefits

| Benefit | Description |
|---------|-------------|
| **Instant Verification** | Response in seconds vs. days with manual process |
| **24/7 Availability** | API accessible round-the-clock from anywhere |
| **Reduced Manual Work** | Automates routine verification requests |
| **Standardized Output** | Consistent JSON format for all consumers |
| **Audit Trail** | Complete record of all verification requests |

### 4.2 Strategic Benefits

- **Compliance** - Meets Government of Nepal digital verification requirements
- **Scalability** - Cloud-native architecture handles growing demand
- **Integration Ready** - Easy to connect with other government systems
- **Future-Proof** - Modern tech stack ensures longevity
- **Cost Efficient** - Reduces staff time spent on manual verification

### 4.3 Expected ROI

- **Staff Time Saved:** 10-15 hours/week on manual verification
- **Processing Cost Reduction:** ~NPR 50,000/month in labor savings
- **Revenue Potential:** Can charge verification fees to external agencies

---

## 5. Scope of Work

### 5.1 In Scope

| Phase | Deliverables |
|-------|--------------|
| **Requirement Analysis** | SRS Document, API Specification, Database Schema |
| **API Development** | Student Verification Endpoint, Transcript Endpoint, Error Handling |
| **Database Setup** | SQL Server Database, Entity Models, Migrations, Indexes |
| **Security Implementation** | Input Validation, SQL Injection Prevention, Rate Limiting |
| **Testing** | Unit Tests, Integration Tests, User Acceptance Testing |
| **Documentation** | API Documentation, User Guide, Deployment Guide |
| **Deployment** | Cloud Infrastructure Setup, SSL Configuration, Go-Live |
| **Support (1 Year)** | Bug Fixes, Security Updates, Performance Optimization |

### 5.2 Out of Scope

- Mobile Application Development
- PDF Transcript Generation (can be added later)
- Digital Signature Implementation
- Integration with external government systems
- Student Portal Development

### 5.3 Future Enhancements (Optional)

| Feature | Estimated Cost |
|---------|----------------|
| PDF Transcript Generation | NPR 25,000 |
| Digital Signature Integration | NPR 30,000 |
| Student Photo API | NPR 15,000 |
| UGC Verification Integration | NPR 40,000 |
| Mobile App (iOS/Android) | NPR 150,000+ |

---

## 6. Technical Architecture

### 6.1 Technology Stack

| Component | Technology | Version |
|-----------|------------|---------|
| API Framework | ASP.NET Core | 10.0 |
| API Style | Minimal API | - |
| ORM | Entity Framework Core | 10.0 |
| Database | SQL Server | 2022+ |
| Cloud Platform | Azure App Service | - |
| Hosting OS | Windows Server (Azure) | - |

### 6.2 System Architecture

```
                    ┌─────────────────┐
                    │   Consumer      │
                    │   Applications  │
                    └────────┬────────┘
                             │
                             ▼
                    ┌─────────────────┐
                    │   Azure API    │
                    │   Management   │
                    └────────┬────────┘
                             │
                             ▼
                    ┌─────────────────┐
                    │  FWU.Nagarik.Api │
                    │   (Azure App    │
                    │    Service)     │
                    └────────┬────────┘
                             │
                             ▼
                    ┌─────────────────┐
                    │   SQL Server    │
                    │   (Azure SQL)  │
                    └─────────────────┘
```

### 6.3 Database Schema

**Students Table**
- RegdNo (Primary Key)
- FirstName, MiddleName, LastName
- DobAD
- ProgramName, IntakeYear
- StudentStatus (Running/Graduated)
- Level, School
- CgpaScore, GraduateYear

**StudentMarks Table**
- Id (Primary Key)
- RegdNo (Foreign Key)
- SubjectName, SubjectCode
- Marks, Grade
- Semester, AcademicYear

**VerificationLogs Table**
- Id (Primary Key)
- RegdNo, Timestamp
- VerificationStatus
- RequestDetails

### 6.4 Security Measures

- Input validation and sanitization
- Parameterized queries (EF Core prevents SQL injection)
- HTTPS enforced with TLS 1.2+
- API key or JWT authentication (optional)
- Rate limiting to prevent abuse
- Azure Defender security monitoring
- Azure Key Vault for secrets management

### 6.5 Performance Considerations

- Database indexes on RegdNo and DobAD
- Connection pooling (EF Core default)
- Async/await patterns throughout
- Response caching for frequently accessed data
- Pagination support for large result sets

---

## 7. Implementation Timeline

### 7.1 Project Phases

| Phase | Duration | Start | End | Deliverables |
|-------|----------|-------|-----|--------------|
| 1. Requirement Analysis | 1 week | Week 1 | Week 1 | SRS, API Spec, DB Schema |
| 2. Database Setup | 1 week | Week 2 | Week 2 | DB Creation, Entities, Seeds |
| 3. API Development | 2 weeks | Week 3 | Week 4 | Endpoints, Business Logic |
| 4. Security & Best Practices | 1 week | Week 5 | Week 5 | Auth, Validation, Logging |
| 5. Testing | 1.5 weeks | Week 6 | Week 7 | Unit Tests, Integration Tests, UAT |
| 6. Deployment | 0.5 weeks | Week 8 | Week 8 | Production Deployment |

**Total Duration: 8 weeks (Expedited Timeline)**

### 7.2 Milestone Schedule

| Milestone | Target Date | Payment |
|-----------|-------------|---------|
| Project Kickoff | April 20, 2026 | 40% |
| UAT Completion | June 10, 2026 | 40% |
| Go-Live | June 15, 2026 | 20% |

---

## 8. Cost Breakdown

### 8.1 One-Time Costs (Development)

| Item | Description | Cost (NPR) |
|------|-------------|------------|
| API Development | Core API with verification & transcript endpoints | 150,000 |
| Database Design | ERD, schema design, entity creation | 10,000 |
| Data Migration | Map and migrate from existing data sources | 15,000 |
| Integration Testing | Unit tests, integration tests | 20,000 |
| Cloud Infrastructure | Azure App Service setup & configuration | 10,000 |
| SSL Certificate | Annual SSL for HTTPS | 1,000 |
| Documentation | API docs, user guides | 5,000 |
| **Subtotal** | | **211,000** |

### 8.2 Recurring Costs (Annual)

| Item | Description | Cost (NPR/year) |
|------|-------------|-----------------|
| Cloud Hosting | Azure App Service (B2 Plan) | 24,000 |
| SQL Server | Azure SQL Database | 16,000 |
| Domain & SSL | Domain renewal + SSL | 5,000 |
| Backup & Monitoring | Automated backups, monitoring | 10,000 |
| **Subtotal** | | **55,000** |

### 8.3 Post-Launch Support (12 Months)

| Item | Description | Cost (NPR) |
|------|-------------|------------|
| Bug Fixes | Critical and non-critical issue resolution | 30,000 |
| Security Updates | Patches and vulnerability fixes | 15,000 |
| Performance Optimization | Query tuning, caching | 10,000 |
| Minor Enhancements | Small feature additions | 20,000 |
| **Subtotal** | | **75,000** |

### 8.4 Total Investment Summary

| Category | Amount (NPR) |
|----------|---------------|
| One-Time Development | 211,000 |
| Annual Recurring | 55,000 |
| 1-Year Support (Included) | 75,000 |
| **First Year Total** | **341,000** |
| **Subsequent Years** | **130,000/year** |

---

## 9. Terms & Conditions

### 9.1 Payment Terms

- **40%** on project kickoff (NPR 84,400)
- **40%** on UAT completion (NPR 84,400)
- **20%** on go-live (NPR 42,200)
- Recurring costs billed quarterly in advance
- Support costs billed annually in advance

### 9.2 Project Assumptions

1. Client will provide access to existing student database
2. Client will provide sample data for testing
3. Client will designate a technical contact for coordination
4. Changes to scope will be handled via change request with additional cost
5. UAT will be completed within 5 business days of delivery

### 9.3 Warranty

- 30-day warranty from go-live for critical bugs
- Bug fixes covered under support package for 12 months
- Security vulnerabilities patched within 48 hours

### 9.4 Intellectual Property

- Source code will be owned by Far Western University
- We retain rights to generic frameworks and tools used
- Documentation created will be handed over to client

### 9.5 Confidentiality

- All project materials treated as confidential
- Non-disclosure agreement will be signed
- Client data will not be shared with third parties

### 9.6 Validity

- This proposal is valid for 30 days from the date
- Prices subject to change after validity period

---

## 10. Contact Information

### Blend Technologies Pvt. Ltd.

| | |
|---|---|
| **Address** | [Company Address] |
| **Phone** | [Phone Number] |
| **Email** | [Email Address] |
| **Website** | [Website] |

### Project Team

| Role | Name | Contact |
|------|------|---------|
| Project Manager | [Name] | [Email] |
| Technical Lead | [Name] | [Email] |
| Quality Assurance | [Name] | [Email] |

---

## Acceptance

This proposal is submitted for your consideration. We are confident that our solution will meet Far Western University's requirements for student verification and transcript services.

**For Blend Technologies Pvt. Ltd.**

_________________________  
Authorized Signature

**Name:** [Authorized Person Name]  
**Title:** [Designation]  
**Date:** ________________

---

*Thank you for considering Blend Technologies Pvt. Ltd. for this project.*