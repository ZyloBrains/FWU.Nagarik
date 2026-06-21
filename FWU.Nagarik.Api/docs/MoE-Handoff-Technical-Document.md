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
4. [Response Field Reference](#4-response-field-reference)
5. [Error Handling](#5-error-handling)
6. [Integration Guide](#6-integration-guide)
7. [Security Notes](#7-security-notes)
8. [Support & Contacts](#8-support--contacts)

---

## 1. Overview

The FWU Nagarik API provides programmatic access to Far Western University's student database for verification purposes. This document describes how to integrate with the **Student Verify** endpoint, which is used for the Ministry of Education's student NOC process.

| Property | Value |
|---|---|
| **Base URL** | `https://emis.fwu.edu.np` |
| **Swagger UI** | `https://emis.fwu.edu.np/docs` |
| **Protocol** | HTTPS (required) |
| **Response Format** | JSON |
| **Authentication** | Two-phase: API Key → JWT Token |

### Key Endpoints

| Method | Endpoint | Auth Required | Description |
|---|---|---|---|
| `GET` | `/api/auth/token` | No | Exchange API key for JWT token |
| `GET` | `/api/student/verify` | Yes (JWT) | Verify student by registration number and DOB |

---

## 2. Authentication

The API uses a **two-phase authentication** flow:

1. **Phase 1**: Exchange your API Key for a JWT (JSON Web Token)
2. **Phase 2**: Use the JWT to access protected endpoints

JWT tokens expire **1 hour** after issuance. You must re-authenticate when the token expires.

### Phase 1: Obtain JWT Token

```
GET /api/auth/token?apiKey=<YOUR_API_KEY>
```

**cURL Example:**

```bash
curl -X GET "https://emis.fwu.edu.np/api/auth/token?apiKey=3ZMCeOs8lpo1eCOVsjUKJqJ8BpmoF9op"
```

**Success Response (200 OK):**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Error Responses:**

| Status | Cause |
|---|---|
| `400 Bad Request` | `apiKey` parameter is missing |
| `401 Unauthorized` | Invalid, inactive, or expired API key |

### Phase 2: Use JWT for Protected Endpoints

Include the JWT in the `Authorization` header using the **Bearer** scheme:

```
Authorization: Bearer <YOUR_JWT_TOKEN>
```

**cURL Example:**

```bash
curl -X GET "https://emis.fwu.edu.np/api/student/verify?registration_number=12345&dobAD=2058-01-15" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

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
  -H "Authorization: Bearer <YOUR_JWT_TOKEN>"
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

## 4. Response Field Reference

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
| `programName` | string | enrolled program name | `"B.Sc. Computer Science"` |
| `intakeYear` | string | Year of admission (Nepali calendar) | `"2078"` |
| `studentStatus` | string | Current status | `"Active"`, `"Graduated"`, `"Discontinued"` |
| `level` | string | Study level | `"Bachelor"`, `"Master"`, `"PhD"` |
| `school` | string | School/department name | `"Central Department of Computer Science"` |
| `cgpaScore` | number | CGPA score (null if not applicable) | `3.45` |
| `graduateYear` | string | Graduation year (null if not graduated) | `"2082"` |

---

## 5. Error Handling

### HTTP Status Codes

| Status | Meaning | Action |
|---|---|---|
| `200 OK` | Student found and verified | Process the `otherData` array |
| `400 Bad Request` | Missing required parameter | Check `registration_number` and `dobAD` are provided |
| `401 Unauthorized` | Authentication failed | Re-authenticate: obtain a new JWT token |
| `404 Not Found` | No student record matches | Verify the registration number and DOB are correct |
| `500 Internal Server Error` | Server-side error | Retry after a brief delay; contact FWU support if persistent |

### Common Error Messages

| Message | Cause | Resolution |
|---|---|---|
| `"apiKey is required"` | Missing `apiKey` query parameter in token request | Include `apiKey` in the request URL |
| `"registration_number is required"` | Missing `registration_number` query parameter | Include `registration_number` in the request URL |
| `"dobAD is required"` | Missing `dobAD` query parameter | Include `dobAD` in the request URL |
| `"No record found for the given registration number / DOB"` | No matching student in database | Double-check registration number and date of birth |

---

## 6. Integration Guide

### Step-by-Step Process

```
┌──────────────┐     ┌──────────────┐     ┌──────────────────┐
│  1. Obtain   │────▶│  2. Verify   │────▶│  3. Process NOC  │
│  JWT Token   │     │  Student     │     │  Decision        │
└──────────────┘     └──────────────┘     └──────────────────┘
```

**Step 1: Obtain JWT Token**

```bash
# Exchange API key for JWT (valid for 1 hour)
curl -X GET "https://emis.fwu.edu.np/api/auth/token?apiKey=3ZMCeOs8lpo1eCOVsjUKJqJ8BpmoF9op"
```

Save the `token` value from the response.

**Step 2: Verify Student**

```bash
# Use JWT to verify student
curl -X GET "https://emis.fwu.edu.np/api/student/verify?registration_number=7401001&dobAD=2058-04-15" \
  -H "Authorization: Bearer <YOUR_JWT_TOKEN>"
```

**Step 3: Process Response**

- If `message` is `"Success"` and `otherData` contains records, the student is verified.
- If status is `404`, the student record was not found — verify the input data.
- If status is `401`, re-authenticate to get a new JWT token.

### Handling Token Expiry

JWT tokens expire after **1 hour**. Implement token refresh logic:

1. Store the JWT and its acquisition timestamp.
2. Before making a verify request, check if the token is older than 55 minutes.
3. If expired or nearing expiry, request a new token using the API key.

---

## 7. Security Notes

| Item | Requirement |
|---|---|
| **API Key** | Keep confidential. Do not expose in client-side code, logs, or public repositories. |
| **JWT Token** | Short-lived (1 hour). Request a new token only when needed. |
| **HTTPS** | All API calls must use HTTPS. HTTP requests will be rejected. |
| **Audit Trail** | All API requests are logged for security and compliance. Include: timestamp, client IP, endpoint accessed, and verification result. |
| **Key Rotation** | Contact FWU to rotate your API key if compromise is suspected. |

---

## 8. Support & Contacts

| Role | Contact |
|---|---|
| **Technical Support** | `[FWU IT Department Email]` |
| **API Issues** | `[FWU Technical Lead Contact]` |
| **Swagger Documentation** | `https://emis.fwu.edu.np/docs` |

---

## Appendix: Quick Reference Card

```
API Key:         3ZMCeOs8lpo1eCOVsjUKJqJ8BpmoF9op
Base URL:        https://emis.fwu.edu.np

TOKEN REQUEST:   GET https://emis.fwu.edu.np/api/auth/token?apiKey=3ZMCeOs8lpo1eCOVsjUKJqJ8BpmoF9op
VERIFY REQUEST:  GET https://emis.fwu.edu.np/api/student/verify?registration_number={REGD}&dobAD={DOB}
AUTH HEADER:     Authorization: Bearer {JWT_TOKEN}
TOKEN EXPIRY:    1 hour
SWAGGER UI:      https://emis.fwu.edu.np/docs
```

---

*Document prepared by Far Western University IT Department*
*For Ministry of Education - Student NOC Verification Integration*
