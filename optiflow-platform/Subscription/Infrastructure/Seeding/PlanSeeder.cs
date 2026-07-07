using Microsoft.Extensions.Logging;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Infrastructure.Seeding;

/// <summary>
///     Ensures the default catalog of subscription plans exists.
///     Without this, "/api/v1/plans" returns an empty list and the "select plan" screen has
///     nothing to render, since there is no other way (UI or otherwise) to create plans today.
///     Safe to run on every startup: <see cref="IPlanCommandService"/> already rejects a plan
///     whose name already exists, so re-running this seeder is a no-op after the first run.
/// </summary>
public static class PlanSeeder
{
    public static async Task SeedAsync(IPlanCommandService planCommandService, ILogger logger, CancellationToken cancellationToken = default)
    {
        // Annual price = 10x the monthly price (2 months free, ≈17% savings), matching the
        // "Ahorra 17%" copy on the frontend's yearly toggle.
        var defaultPlans = new[]
        {
            new CreatePlanCommand("Básico Mensual", new SubscriptionTier(SubscriptionTier.Basic), 49m,
                "Ideal para ópticas pequeñas que recién digitalizan su gestión. Incluye pacientes, ventas e inventario básico."),
            new CreatePlanCommand("Básico Anual", new SubscriptionTier(SubscriptionTier.Basic), 490m,
                "Ideal para ópticas pequeñas que recién digitalizan su gestión. Incluye pacientes, ventas e inventario básico."),

            new CreatePlanCommand("Profesional Mensual", new SubscriptionTier(SubscriptionTier.Professional), 99m,
                "Para ópticas en crecimiento: órdenes de laboratorio, reportes avanzados y múltiples usuarios de staff."),
            new CreatePlanCommand("Profesional Anual", new SubscriptionTier(SubscriptionTier.Professional), 990m,
                "Para ópticas en crecimiento: órdenes de laboratorio, reportes avanzados y múltiples usuarios de staff."),

            new CreatePlanCommand("Empresarial Mensual", new SubscriptionTier(SubscriptionTier.Enterprise), 199m,
                "Para cadenas de ópticas: todas las funcionalidades, soporte prioritario y usuarios ilimitados."),
            new CreatePlanCommand("Empresarial Anual", new SubscriptionTier(SubscriptionTier.Enterprise), 1990m,
                "Para cadenas de ópticas: todas las funcionalidades, soporte prioritario y usuarios ilimitados."),
        };

        foreach (var command in defaultPlans)
        {
            try
            {
                var result = await planCommandService.Handle(command, cancellationToken);
                if (result.IsFailure)
                    logger.LogInformation("Plan seed skipped for '{Name}': already exists.", command.Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to seed plan '{Name}'.", command.Name);
            }
        }
    }
}
