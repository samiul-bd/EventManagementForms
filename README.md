# Event Management System (Desktop & Crystal Reports)

A comprehensive desktop application designed to streamline the planning and organization of professional events, client records, and detailed program schedules, featuring enterprise-level reporting capabilities.

## What This Project Does

Managing events often involves juggling multiple schedules, tracking budgets, and keeping client information organized. This application provides a straightforward digital workspace for event organizers to handle everything from initial client registration to multi-day scheduling. It eliminates manual paperwork by allowing users to instantly generate, preview, and export complete event dossiers as professional printable reports.

## Key Features

*   **Event Creation and Budgeting:** Easily input event titles, set start and end dates, define budgets, and assign events to specific clients.
*   **Client Management:** Maintain a clean registry of customers and their contact numbers, ensuring event records are always linked to the correct client profile.
*   **Multi-Day Program Scheduling:** Break down larger events into specific timed programs or activities, allowing organizers to manage sub-schedules and durations efficiently.
*   **Centralized Overview:** View all registered events and their corresponding sub-programs in a clear, master-detail layout that lets you track schedules and make updates instantly.
*   **Professional Reporting:** Generate comprehensive printable reports containing master-detail event information, client details, and embedded profile images.

## Key Technical Features

*   **Crystal Reports Integration:** Utilizes `CrystalDecisions` libraries, strongly-typed XML schemas (`dsEventInfo.xsd`), and a custom Report Viewer form for robust document generation and export (PDF/Excel).
*   **Raw ADO.NET Architecture:** Executes explicit, parameterized SQL queries using `SqlConnection`, `SqlCommand`, and `SqlDataAdapter` for high-performance data access without relying on an ORM.
*   **Transactional Integrity:** Implements explicit database transaction management (`Commit`/`Rollback` blocks) to ensure relational data consistency when saving or deleting complex parent-child event structures.
*   **Master-Detail Data Entry:** Seamlessly manages parent event details and child program schedules simultaneously on a single unified interface.
*   **Media Handling:** Includes image uploading, binary conversion, and database storage for event profiles, which dynamically render in both the UI and the generated Crystal Reports.

## Technology Stack

*   **Language & Framework:** C#, .NET Framework (Windows Forms)
*   **Data Access:** ADO.NET 
*   **Reporting:** SAP Crystal Reports
*   **Database:** MS SQL Server

## Setup and Installation

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/samiul-bd/EventManagementForms.git](https://github.com/samiul-bd/EventManagementForms.git)
    ```
2.  **Database Setup:**
    *   Open SQL Server Management Studio (SSMS).
    *   Execute the `DBScript.txt` file located in the root directory to generate the schema and tables.
3.  **Configuration:**
    *   Open `App.config` in Visual Studio and update the connection string to point to your local SQL Server instance.
4.  **Reporting Dependencies:**
    *   *Note: To preview or print the reports locally, the Crystal Reports Runtime Engine (v13.x) must be installed on your Windows machine.*
5.  **Run:**
    *   Build and run the project using Visual Studio (F5).
