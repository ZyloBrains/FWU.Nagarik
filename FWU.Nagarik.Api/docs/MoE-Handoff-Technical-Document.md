# FWU Nagarik API - Technical Handoff Document

**Prepared for**: Ministry of Education, Nepal
**Purpose**: Student NOC (No Objection Certificate) Verification
**Institution**: Far Western University (FWU)
**API Version**: v1
**Date**: June 2026

---

## Table of Contents

1. [Overview](#1-overview)
2. [Authentication](#2-authentication)
3. [Student Verify Endpoint](#3-student-verify-endpoint)
4. [Student Transcript Endpoint](#4-student-transcript-endpoint)
5. [Response Field Reference](#5-response-field-reference)
6. [Error Handling](#6-error-handling)
7. [Integration Guide](#7-integration-guide)
8. [Security Notes](#8-security-notes)
9. [Support & Contacts](#9-support--contacts)

---

## 1. Overview

The FWU Nagarik API provides programmatic access to Far Western University's student database for verification purposes. This document describes how to integrate with the **Student Verify** and **Student Transcript** endpoints, which are used for the Ministry of Education's student NOC process.

| Property | Value |
|---|---|
| **Base URL** | `https://emis.fwu.edu.np` |
| **Swagger UI** | `https://emis.fwu.edu.np/docs` |
| **Protocol** | HTTPS (required) |
| **Response Format** | JSON |
| **Authentication** | API Key (via `X-Api-Key` header) |

### Key Endpoints

| Method | Endpoint | Auth Required | Description |
|---|---|---|---|
| `GET` | `/api/student/verify` | Yes (API Key) | Verify student by registration number and DOB |
| `GET` | `/api/student/transcript` | Yes (API Key) | Retrieve student transcript |

---

## 2. Authentication

The API uses **API Key authentication**. Include your API key in the `X-Api-Key` header with every request.

```
X-Api-Key: <YOUR_API_KEY>
```

API keys are validated against the server database. Each key can be:
- **Named** and assigned to an **organization**
- **Activated** or **deactivated** at any time
- **Set with an optional expiry date**

Contact FWU to obtain your API key.

---

## 3. Student Verify Endpoint

### Request

```
GET /api/student/verify
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `registration_number` | string | Yes | Student's FWU registration number |
| `dobAD` | string | Yes | Date of birth in Nepali calendar (Bikram Sambat), format: `YYYY-MM-DD` |

**Full cURL Example:**

```bash
curl -X GET "https://emis.fwu.edu.np/api/student/verify?registration_number=7401001&dobAD=2058-04-15" \
  -H "X-Api-Key: YOUR_API_KEY_HERE"
```

### Response

**200 OK - Student Found:**

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

**404 Not Found:**

```json
{
  "message": "No record found for the given registration number / DOB"
}
```

**400 Bad Request (missing parameter):**

```json
{
  "message": "registration_number is required"
}
```

or

```json
{
  "message": "dobAD is required"
}
```

> **Note**: A student may have multiple records in `otherData` if enrolled in multiple programs.

---

## 4. Student Transcript Endpoint

### Request

```
GET /api/student/transcript
```

**Query Parameters:**

| Parameter | Type | Required | Description |
|---|---|---|---|
| `registration_number` | string | Yes | Student's FWU registration number |
| `dobAD` | string | Yes | Date of birth in Nepali calendar (Bikram Sambat), format: `YYYY-MM-DD` |

**Full cURL Example:**

```bash
curl -X GET "https://emis.fwu.edu.np/api/student/transcript?registration_number=7401001&dobAD=2058-04-15" \
  -H "X-Api-Key: YOUR_API_KEY_HERE"
```

### Response

**200 OK - Transcript Found:**

Returns the student's transcript data including semesters, subjects, and grades.

**404 Not Found:**

```json
{
  "message": "No record found for the given registration number / DOB"
}
```

---

## 5. Response Field Reference

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

---

## 6. Error Handling

### HTTP Status Codes

| Status | Meaning | Action |
|---|---|---|
| `200 OK` | Student found and verified | Process the `otherData` array |
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

---

## 7. Integration Guide

### Step-by-Step Process

```
┌──────────────┐     ┌──────────────┐     ┌──────────────────┐
│  1. Verify   │────▶│  2. Process  │────▶│  3. Process NOC  │
│  Student     │     │  Response    │     │  Decision        │
└──────────────┘     └──────────────┘     └──────────────────┘
```

**Step 1: Verify Student**

```bash
# Verify student with API key
curl -X GET "https://emis.fwu.edu.np/api/student/verify?registration_number=7401001&dobAD=2058-04-15" \
  -H "X-Api-Key: YOUR_API_KEY_HERE"
```

**Step 2: Process Response**

- If `message` is `"Success"` and `otherData` contains records, the student is verified.
- If status is `404`, the student record was not found — verify the input data.
- If status is `401`, check that your API key is valid and active.

---

## 8. Security Notes

| Item | Requirement |
|---|---|
| **API Key** | Keep confidential. Do not expose in client-side code, logs, or public repositories. |
| **HTTPS** | All API calls must use HTTPS. HTTP requests will be rejected. |
| **Audit Trail** | All API requests are logged for security and compliance. Include: timestamp, client IP, endpoint accessed, and verification result. |
| **Key Rotation** | Contact FWU to rotate your API key if compromise is suspected. |

---

## 9. Support & Contacts

| Role | Contact |
|---|---|
| **Technical Support** | `[FWU IT Department Email]` |
| **API Issues** | `[FWU Technical Lead Contact]` |
| **Swagger Documentation** | `https://emis.fwu.edu.np/docs` |

---

## Appendix: Quick Reference Card

```
API Key:         YOUR_API_KEY_HERE (obtained from FWU)
Base URL:        https://emis.fwu.edu.np
AUTH HEADER:     X-Api-Key: YOUR_API_KEY_HERE

VERIFY REQUEST:  GET https://emis.fwu.edu.np/api/student/verify?registration_number={REGD}&dobAD={DOB}
TRANSCRIPT REQ:  GET https://emis.fwu.edu.np/api/student/transcript?registration_number={REGD}&dobAD={DOB}
SWAGGER UI:      https://emis.fwu.edu.np/docs
```

---

*Document prepared by Far Western University IT Department*
*For Ministry of Education - Student NOC Verification Integration*
