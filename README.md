# LITESTAFF

## Human Resource Information and Payroll System

A comprehensive and modern Human Resource Information and Payroll System designed to streamline HR processes, empower employees, and provide robust data management. This system is built on a powerful and scalable technology stack, offering a seamless experience for HR administrators and employees alike.

## ✨ Overview

This project aims to simplify the complexities of human resource management by providing an all-in-one solution. From detailed employee record-keeping to precise attendance tracking and integrated payroll processing, this system is engineered for efficiency. A key feature is the mobile-first approach, featuring a dedicated Employee Self-Service (ESS) portal and cutting-edge mobile face recognition for biometric attendance, eliminating the need for traditional hardware.

This repository contains the source code for the backend services, the mobile application, and all related components.

## ⭐ Key Features

### For HR Administrators:

  * **Centralized Employee Records Management:** Maintain a comprehensive and secure database of all employee information, including personal details, employment history, contract details, and performance records.
  * **Automated Attendance and Timekeeping:** Track employee work hours, overtime, and leaves with precision. Our system supports various attendance recording methods, including the innovative mobile face recognition feature.
  * **Integrated & Accurate Payroll System:** Seamlessly calculate salaries, deductions, taxes, and benefits. The integrated nature of the system ensures that payroll is always based on up-to-date attendance and employee data, minimizing errors and saving time.
  * **Powerful Reporting & Analytics:** Generate insightful reports for compliance, strategic planning, and performance analysis using the robust DevExpress Reports.

### For Employees:

  * **Employee Self-Service (ESS) Mobile Portal:** Empower your workforce with on-the-go access to their personal information. Employees can:
      * View their payslips and salary history.
      * File for leaves and check leave balances.
      * View their daily time records.
      * Update their personal information.
  * **Mobile Face Recognition Biometrics:** A convenient and hygienic way to clock in and out using their smartphone's camera. The system leverages OpenCV for accurate and secure facial recognition, ensuring the right employee is at the right place.

## 🚀 Technology Stack

This system is built using a modern, robust, and scalable set of technologies to ensure high performance, maintainability, and a feature-rich user experience.

  * **Backend:**
      * **ASP.NET Core:** Serves as the high-performance, cross-platform foundation for all backend services, APIs, and business logic.
      * **Entity Framework (EF) Core:** The primary Object-Relational Mapper (ORM) for streamlined and predictable data access to the main SQL Server database.
      * **Dapper:** A high-performance micro-ORM leveraged for complex, performance-critical database queries, ensuring rapid data retrieval for reporting and analytics.
      * **In-Memory Caching:** Implemented for the AI Chat Assistant to ensure chat persistence and deliver instantaneous responses, significantly enhancing performance and user experience.
  * **Database:**
      * **SQL Server:** The primary relational database for robust, secure, and scalable storage of all core HR and payroll data.
      * **SQLite:** Utilized as a lightweight, local database on mobile devices for offline storage capabilities, such as caching employee data for quick access.
  * **Mobile Applications:**
      * **Xamarin:** Powers the main Employee Self-Service (ESS) portal. This allows employees to access their individual Daily Time Records (DTR) and payslips, as well as use fingerprint-based biometric authentication on supported devices.
      * **.NET MAUI:** Specifically used to build the modern, cross-platform UI for the dedicated face recognition biometric scanning kiosk or application, providing a seamless and responsive user experience.
  * **AI & Biometrics:**
      * **OpenCV:** The core computer vision library used for processing image data. We have coded custom algorithms for inferencing, leveraging OpenCV's power to handle the real-time demands of facial recognition.
      * **ArcFace Model:** A state-of-the-art deep learning model used for highly accurate and secure facial recognition. This model is integrated into our system to ensure precise and reliable biometric attendance tracking.
      * **Gemini (via Google Generative AI SDK):** Powers the intelligent AI Chat Assistant within the system. By connecting to Google's powerful Gemini models through the official SDK, the assistant can handle HR-related queries, provide instant support, and guide users through various processes.
  * **UI & Reporting:**
      * **Telerik Components:** A suite of high-quality, pre-built UI components used within the Xamarin application to accelerate development and provide a rich, polished user experience for the employee portal.
      * **DevExpress Reports:** A powerful reporting engine used to generate detailed, customizable, and professional reports for payroll, attendance, compliance, and HR analytics directly from the backend.

## 🛠️ Getting Started

To get a local copy up and running, please follow these steps.

### Prerequisites

  * [.NET SDK](https://dotnet.microsoft.com/download)
  * [SQL Server(2019 or later)](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
  * [Visual Studio(2022)](https://visualstudio.microsoft.com/) with the following workloads:
      * ASP.NET and web development
      * .NET Multi-platform App UI development (for MAUI)
      * Xamarin
  * VSCode

### Installation

1.  **Clone the repository:**
    ```sh
    git clone https://github.com/ewceniza9009/litestaff.git
    ```
2.  **Backend Setup:**
      * Navigate to the backend solution file (`.sln`).
      * Update the database connection string in `appsettings.json`.
      * Run the database migrations:
        ```sh
        dotnet ef database update
        ```
      * Build and run the backend project.
3.  **Mobile App Setup:**
      * Open the mobile application solution file (`.sln`).
      * Restore the NuGet packages.
      * Update the API endpoint in the mobile app's configuration file to point to your local backend URL.
      * Build and deploy the application to your emulator or physical device.
  
## appsettings.json Setup

To use the application, copy the following configuration into your `appsettings.json` file and replace the placeholder values with your actual credentials.

```json
{
  "DeviceIdToken": "YOUR_DEVICE_ID_TOKEN",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=[SERVER];Initial Catalog=[DB_NAME];Integrated Security=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Gemini": {
    "ApiKey": "GEMINI_API_KEY_1"
  },
  "Gemini2": {
    "ApiKey": "GEMINI_API_KEY_2"
  },
  "AllowedHosts": "*"
}
```

## 📸 Screenshots

![image](https://github.com/user-attachments/assets/0c4c52e4-92e7-4bda-b312-369f17955b85)
![image](https://github.com/user-attachments/assets/d224eba4-0852-4806-b4cc-50c031c770af)
![image](https://github.com/user-attachments/assets/0b76d89f-fde0-42ee-8c1a-8601f85c5f23)
![image](https://github.com/user-attachments/assets/2ecfbc00-ffdb-4cdf-bc4c-8fa5d74dca62)
![image](https://github.com/user-attachments/assets/8dc4e4a3-1a46-4a8e-abb3-89097b1b70ea)
![image](https://github.com/user-attachments/assets/4896c1ff-76c1-4bb4-b80f-f3b5ee894ee8)
![image](https://github.com/user-attachments/assets/711bce56-6e2b-4fce-8556-4c671fceed63)
![image](https://github.com/user-attachments/assets/fa989ee3-376f-48a6-a848-6aa58b196b43)

## 🤝 Contributing

Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1.  Fork the Project
2.  Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3.  Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4.  Push to the Branch (`git push origin feature/AmazingFeature`)
5.  Open a Pull Request

## 📜 License

Distributed by Erwin Wilson Ceniza.

## 📞 Contact

**Erwin Wilson Ceniza** - [erwinwilsonceniza@gmail.com](mailto:erwinwilsonceniza@gmail.com)

**Project Link:** [https://github.com/ewceniza9009/litestaff](https://www.google.com/search?q=https://github.com/ewceniza9009/litestaff)
