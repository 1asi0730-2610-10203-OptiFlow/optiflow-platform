using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace optiflow_platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `analytics_reports` (
                `id` int NOT NULL AUTO_INCREMENT,
                `generated_by` varchar(255) NOT NULL,
                `period` varchar(7) NOT NULL,
                `generated_at` datetime(6) NOT NULL,
                `total_revenue` decimal(15,2) NOT NULL,
                `total_transactions` int NOT NULL,
                `conversion_rate` decimal(5,4) NOT NULL,
                `average_delivery_days` decimal(6,2) NOT NULL,
                `on_time_delivery_rate` decimal(5,4) NOT NULL,
                `rework_rate` decimal(5,4) NOT NULL,
                `total_orders` int NOT NULL,
                `pending_balance0_to7` decimal(15,2) NOT NULL,
                `pending_balance8_to15` decimal(15,2) NOT NULL,
                `pending_balance16_to30` decimal(15,2) NOT NULL,
                `pending_balance_over30` decimal(15,2) NOT NULL,
                PRIMARY KEY (`id`)
            ) CHARACTER SET utf8mb4;");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `categories` (
                `id` int NOT NULL AUTO_INCREMENT,
                `name` varchar(255) NOT NULL,
                PRIMARY KEY (`id`)
            ) CHARACTER SET utf8mb4;");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `laboratories` (
                `id` int NOT NULL AUTO_INCREMENT,
                `name` varchar(255) NOT NULL,
                `contact_phone` varchar(50) NOT NULL,
                `contact_email` varchar(255) NOT NULL,
                PRIMARY KEY (`id`),
                UNIQUE KEY `i_x_laboratories_name` (`name`)
            ) CHARACTER SET utf8mb4;");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `patients` (
                `id` int NOT NULL AUTO_INCREMENT,
                `first_name` varchar(100) NOT NULL,
                `last_name` varchar(100) NOT NULL,
                `dni` varchar(20) NOT NULL,
                `phone` varchar(20) NULL,
                `email` varchar(255) NULL,
                `birth_date` date NOT NULL,
                PRIMARY KEY (`id`),
                UNIQUE KEY `i_x_patients_dni` (`dni`)
            ) CHARACTER SET utf8mb4;");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `payments` (
                `id` int NOT NULL AUTO_INCREMENT,
                `sale_id` int NOT NULL,
                `total_amount` decimal(10,2) NOT NULL,
                `paid_amount` decimal(10,2) NOT NULL,
                `outstanding_balance` decimal(10,2) NOT NULL,
                `status` varchar(20) NOT NULL,
                PRIMARY KEY (`id`)
            ) CHARACTER SET utf8mb4;");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `products` (
                `id` int NOT NULL AUTO_INCREMENT,
                `category_id` int NOT NULL,
                `category` varchar(255) NOT NULL,
                `supplier_id` int NOT NULL,
                `supplier_name` varchar(255) NOT NULL,
                `sku` varchar(100) NOT NULL,
                `name` varchar(255) NOT NULL,
                `brand` varchar(255) NOT NULL,
                `model` varchar(255) NOT NULL,
                `price` decimal(10,2) NOT NULL,
                `stock` int NOT NULL,
                `minimum_stock_threshold` int NOT NULL,
                `last_restock_date` varchar(50) NOT NULL,
                PRIMARY KEY (`id`),
                UNIQUE KEY `i_x_products_sku` (`sku`)
            ) CHARACTER SET utf8mb4;");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `sales` (
                `id` int NOT NULL AUTO_INCREMENT,
                `invoice_number` varchar(50) NOT NULL,
                `lab_order_number` varchar(50) NOT NULL,
                `patient_id` int NOT NULL,
                `patient_name` varchar(255) NOT NULL,
                `user_id` int NOT NULL,
                `user_name` varchar(255) NOT NULL,
                `total_amount` decimal(10,2) NOT NULL,
                `advance` decimal(10,2) NOT NULL,
                `pending_balance` decimal(10,2) NOT NULL,
                `discount_code` varchar(50) NOT NULL,
                `discount_amount` decimal(10,2) NOT NULL,
                `payment_method` varchar(50) NOT NULL,
                `status` varchar(50) NOT NULL,
                `created_at` varchar(50) NOT NULL,
                `delivered_at` varchar(50) NOT NULL,
                `notes` varchar(1000) NOT NULL,
                PRIMARY KEY (`id`)
            ) CHARACTER SET utf8mb4;");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `stock_audit_logs` (
                `id` int NOT NULL AUTO_INCREMENT,
                `product_id` int NOT NULL,
                `product_name` varchar(255) NOT NULL,
                `sku` varchar(100) NOT NULL,
                `operation` varchar(50) NOT NULL,
                `previous_stock` int NOT NULL,
                `quantity` int NOT NULL,
                `new_stock` int NOT NULL,
                `author` varchar(255) NOT NULL,
                `date` varchar(20) NOT NULL,
                `time` varchar(20) NOT NULL,
                PRIMARY KEY (`id`)
            ) CHARACTER SET utf8mb4;");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `subscription_billings` (
                `id` int NOT NULL AUTO_INCREMENT,
                `subscription_id` int NOT NULL,
                `renewal_date` datetime NOT NULL,
                `auto_renew` tinyint(1) NOT NULL,
                `billing_status` varchar(50) NOT NULL,
                PRIMARY KEY (`id`)
            ) CHARACTER SET utf8mb4;");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `subscription_payments` (
                `id` int NOT NULL AUTO_INCREMENT,
                `subscription_id` int NOT NULL,
                `amount` decimal(10,2) NOT NULL,
                `payment_method` varchar(50) NOT NULL,
                `status` varchar(50) NOT NULL,
                `processed_at` datetime NULL,
                PRIMARY KEY (`id`)
            ) CHARACTER SET utf8mb4;");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `subscription_plans` (
                `id` int NOT NULL AUTO_INCREMENT,
                `name` varchar(100) NOT NULL,
                `tier` varchar(50) NOT NULL,
                `price` decimal(10,2) NOT NULL,
                `description` varchar(500) NOT NULL,
                PRIMARY KEY (`id`)
            ) CHARACTER SET utf8mb4;");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `subscriptions` (
                `id` int NOT NULL AUTO_INCREMENT,
                `admin_id` int NOT NULL,
                `plan_id` int NOT NULL,
                `tier` varchar(50) NOT NULL,
                `amount` decimal(10,2) NOT NULL,
                `payment_method` varchar(50) NOT NULL,
                `status` varchar(50) NOT NULL,
                `start_date` datetime NULL,
                `end_date` datetime NULL,
                PRIMARY KEY (`id`)
            ) CHARACTER SET utf8mb4;");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `suppliers` (
                `id` int NOT NULL AUTO_INCREMENT,
                `name` varchar(255) NOT NULL,
                `contact_person` varchar(255) NOT NULL,
                `phone` varchar(50) NOT NULL,
                `email` varchar(255) NOT NULL,
                PRIMARY KEY (`id`)
            ) CHARACTER SET utf8mb4;");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `work_orders` (
                `id` int NOT NULL AUTO_INCREMENT,
                `sale_id` int NOT NULL,
                `recipe_id` int NOT NULL,
                `laboratory_id` int NOT NULL,
                `status` varchar(50) NOT NULL,
                `priority` varchar(20) NOT NULL,
                `patient_name` varchar(255) NOT NULL,
                `laboratory_name` varchar(255) NOT NULL,
                `lens_type` varchar(100) NOT NULL,
                `frame` varchar(255) NOT NULL,
                `prescription` varchar(500) NOT NULL,
                `delivery_date` varchar(50) NOT NULL,
                `deposit` decimal(10,2) NOT NULL,
                `total` decimal(10,2) NOT NULL,
                `is_rework` tinyint(1) NOT NULL,
                PRIMARY KEY (`id`)
            ) CHARACTER SET utf8mb4;");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `staff_metrics` (
                `id` int NOT NULL AUTO_INCREMENT,
                `report_id` int NOT NULL,
                `employee_name` varchar(255) NOT NULL,
                `quotations_issued` int NOT NULL,
                `sales_closed` int NOT NULL,
                `total_revenue` decimal(15,2) NOT NULL,
                PRIMARY KEY (`id`),
                KEY `i_x_staff_metrics_report_id` (`report_id`),
                CONSTRAINT `f_k_staff_metrics_analytics_reports_report_id`
                    FOREIGN KEY (`report_id`) REFERENCES `analytics_reports` (`id`) ON DELETE CASCADE
            ) CHARACTER SET utf8mb4;");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `clinical_records` (
                `id` int NOT NULL AUTO_INCREMENT,
                `patient_id` int NOT NULL,
                PRIMARY KEY (`id`),
                UNIQUE KEY `i_x_clinical_records_patient_id` (`patient_id`),
                CONSTRAINT `f_k_clinical_records_patients_patient_id`
                    FOREIGN KEY (`patient_id`) REFERENCES `patients` (`id`) ON DELETE CASCADE
            ) CHARACTER SET utf8mb4;");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `prescriptions` (
                `id` int NOT NULL AUTO_INCREMENT,
                `clinical_record_id` int NOT NULL,
                `od_sphere` decimal(5,2) NOT NULL,
                `od_cylinder` decimal(5,2) NOT NULL,
                `od_axis` int NOT NULL,
                `oi_sphere` decimal(5,2) NOT NULL,
                `oi_cylinder` decimal(5,2) NOT NULL,
                `oi_axis` int NOT NULL,
                `addition` decimal(5,2) NULL,
                `notes` varchar(1000) NOT NULL,
                `doctor_name` varchar(255) NOT NULL,
                `created_at` datetime(6) NOT NULL,
                PRIMARY KEY (`id`),
                KEY `i_x_prescriptions_clinical_record_id` (`clinical_record_id`),
                CONSTRAINT `f_k_prescriptions_clinical_records_clinical_record_id`
                    FOREIGN KEY (`clinical_record_id`) REFERENCES `clinical_records` (`id`) ON DELETE CASCADE
            ) CHARACTER SET utf8mb4;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "laboratories");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "prescriptions");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "sales");

            migrationBuilder.DropTable(
                name: "staff_metrics");

            migrationBuilder.DropTable(
                name: "stock_audit_logs");

            migrationBuilder.DropTable(
                name: "subscription_billings");

            migrationBuilder.DropTable(
                name: "subscription_payments");

            migrationBuilder.DropTable(
                name: "subscription_plans");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "suppliers");

            migrationBuilder.DropTable(
                name: "work_orders");

            migrationBuilder.DropTable(
                name: "clinical_records");

            migrationBuilder.DropTable(
                name: "analytics_reports");

            migrationBuilder.DropTable(
                name: "patients");
        }
    }
}
