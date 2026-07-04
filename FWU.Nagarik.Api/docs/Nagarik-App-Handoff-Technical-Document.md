# FWU Nagarik API - Technical Handoff Document (Nagarik App)

**Prepared for**: Nagarik App Team
**Purpose**: Student Verification & Transcript PDF Integration
**Institution**: Far Western University (FWU)
**API Version**: v1
**Date**: July 2026

---

## Table of Contents

1. [Overview](#1-overview)
2. [Response Field Reference](#2-response-field-reference)
3. [Error Handling](#3-error-handling)
4. [Security Notes](#4-security-notes)
5. [Support & Contacts](#5-support--contacts)

---

## 1. Overview

The FWU Nagarik API provides programmatic access to Far Western University's student database. This document describes how the **Nagarik App** mobile application can integrate with the **Student Verify** and **Student Transcript PDF** endpoints.

| Property | Value |
|---|---|
| **Base URL** | `https://emis.fwu.edu.np` |
| **Swagger UI** | `https://emis.fwu.edu.np/docs` |
| **Protocol** | HTTPS (required) |
| **Response Format** | JSON (Verify) / PDF Binary (Transcript) |
| **Authentication** | API Key (via `X-Api-Key` header) |

### Key Endpoints

| Method | Endpoint | Auth Required | Response | Description |
|---|---|---|---|---|
| `GET` | `/api/student/verify` | Yes (API Key) | JSON | Verify student by registration number and DOB |
| `GET` | `/api/student/transcript` | Yes (API Key) | **PDF** | Retrieve student transcript as a downloadable PDF |

### Verify Endpoint Response (JSON)

```json
{
  "data": "7401001",
  "message": "Success",
  "otherData": [
    {
      "regdNo": "7401001",
      "firstName": "Ram",
      "middleName": "Bahadur",
      "lastName": "Thapa",
      "dobAD": "2058-04-15",
      "programName": "B.Sc. Computer Science",
      "intakeYear": "2078",
      "studentStatus": "Active",
      "level": "Bachelor",
      "school": "Central Department of Computer Science",
      "cgpaScore": 3.45,
      "graduateYear": "2082"
    }
  ]
}
```

### Transcript Endpoint Response (PDF Binary)

| Property | Value |
|---|---|
| **Content-Type** | `application/pdf` |
| **Content-Disposition** | `attachment; filename="{registration_number}_Transcript.pdf"` |
| **Body** | Binary PDF (A4 format) |


## 2. Response Field Reference

### Verify Endpoint

| Field | Type | Description |
|---|---|---|
| `data` | string | Registration number (echoed back) |
| `message` | string | Status message (`Success` or error description) |
| `otherData` | array | Array of student records matching the query |

### Student Data Fields

| Field | Type | Description | Example |
|---|---|---|---|
| `regdNo` | string | FWU registration number | `"7401001"` |
| `firstName` | string | Student's first name | `"Ram"` |
| `middleName` | string | Student's middle name | `"Bahadur"` |
| `lastName` | string | Student's last name | `"Thapa"` |
| `dobAD` | string | Date of birth (Nepali calendar) | `"2058-04-15"` |
| `programName` | string | Enrolled program name | `"B.Sc. Computer Science"` |
| `intakeYear` | string | Year of admission (Nepali calendar) | `"2078"` |
| `studentStatus` | string | Current status | `"Active"`, `"Graduated"`, `"Discontinued"` |
| `level` | string | Study level | `"Bachelor"`, `"Master"`, `"PhD"` |
| `school` | string | School/department name | `"Central Department of Computer Science"` |
| `cgpaScore` | number | CGPA score (null if not applicable) | `3.45` |
| `graduateYear` | string | Graduation year (null if not graduated) | `"2082"` |

### Transcript Endpoint

The transcript endpoint returns a **raw PDF binary**, not JSON. The PDF contains:

| Section | Content |
|---|---|
| **Header** | University name, office name, location |
| **Student Info** | Name, registration number, faculty, program, campus |
| **Semester Tables** | Subject codes, names, credit hours, grades, grade points |
| **Summary** | Total credit hours, total grade points, CGPA |
| **Footer** | Issue date, serial number |

## 3. Error Handling

### HTTP Status Codes

| Status | Meaning | Action |
|---|---|---|
| `200 OK` | Success | Verify: process `otherData` / Transcript: save PDF bytes |
| `400 Bad Request` | Missing required parameter | Check `registration_number` and `dobAD` are provided |
| `401 Unauthorized` | Authentication failed | Verify your API key is valid and active |
| `404 Not Found` | No student record matches | Verify the registration number and DOB are correct |
| `500 Internal Server Error` | Server-side error | Retry after a brief delay; contact FWU support if persistent |

### Common Error Messages

| Message | Cause | Resolution |
|---|---|---|
| `"registration_number is required"` | Missing `registration_number` query parameter | Include `registration_number` in the request URL |
| `"dobAD is required"` | Missing `dobAD` query parameter | Include `dobAD` in the request URL |
| `"No record found for the given registration number / DOB"` | No matching student in database | Double-check registration number and date of birth |
| `"Invalid API key."` | Missing or unrecognized API key in header | Include a valid `X-Api-Key` header |
| `"API key has expired."` | The API key has passed its expiry date | Contact FWU to obtain a new key |


## 4. Security Notes

| Item | Requirement |
|---|---|
| **API Key** | Keep confidential. Do not expose in client-side code, logs, or public repositories. Store securely in app keychain/keystore. |
| **HTTPS** | All API calls must use HTTPS. HTTP requests will be rejected. |
| **Audit Trail** | All API requests are logged for security and compliance. Includes: timestamp, client IP, endpoint accessed, and verification result. |
| **Key Rotation** | Contact FWU to rotate your API key if compromise is suspected. |
| **PDF Caching** | Consider caching PDFs locally with a TTL to reduce redundant API calls. |
| **Rate Limiting** | Be mindful of request volume. Cache verify results where appropriate. |

## 5. Support & Contacts

| Role | Contact |
|---|---|
| **Technical Support** | `Prakash Bdr Saud, IT Section FWU, prakesh@fwu.edu.np  9841563644` |
| **API Issues** | `Bishnu Rawal: 9851345885, Jagdish Bhatta: 9841563644` |
| **Swagger Documentation** | `https://emis.fwu.edu.np/docs` |

---

## Appendix: Quick Reference Card

```
API Key:         7QlhvAcjugXVUeQ6qD0GLxlRFFJMLACP
Base URL:        https://emis.fwu.edu.np
AUTH HEADER:     X-Api-Key: 7QlhvAcjugXVUeQ6qD0GLxlRFFJMLACP

VERIFY REQUEST:  GET https://emis.fwu.edu.np/api/student/verify?registration_number={REGD}&dobAD={DOB}
                 Response: JSON { data, message, otherData[] }

TRANSCRIPT REQ:  GET https://emis.fwu.edu.np/api/student/transcript?registration_number={REGD}&dobAD={DOB}
                 Response: PDF binary (Content-Type: application/pdf)

SWAGGER UI:      https://emis.fwu.edu.np/docs
```

---

*Document prepared by Far Western University IT Department*
*For Nagarik App - Student Verification & Transcript PDF Integration*
