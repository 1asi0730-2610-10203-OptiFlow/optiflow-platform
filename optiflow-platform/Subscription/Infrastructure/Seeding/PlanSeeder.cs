using System.Linq;
using Microsoft.Extensions.Logging;
using optiflow_platform.Shared.Domain.Repositories;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;
using optiflow_platform.Subscription.Domain.Repositories;

namespace optiflow_platform.Subscription.Infrastructure.Seeding;

/// <summary>
///     Ensures the default catalog of subscription plans exists and is the single source of truth.
///     Without this, "/api/v1/plans" returns an empty list and the "select plan" screen has
///     nothing to render, since there is no other way (UI or otherwise) to create plans today.
///     Safe to run on every startup: it first prunes any plan that is not part of the canonical
///     catalog (e.g. legacy/duplicate plans previously created by demo seed scripts under different
///     names or prices) and then re-creates the canonical plans, which <see cref="IPlanCommandService"/>
///     already treats as a no-op when a plan with the same name already exists.
/// </summary>
public static class PlanSeeder
{
    public static async Task SeedAsync(IPlanCommandService planCommandService, IPlanRepository planRepository,
        IUnitOfWork unitOfWork, ILogger logger, CancellationToken cancellationToken = default)
    {
        // Annual price = 10x the monthly price (2 months free, ≈17% savings), matching the
        // "Ahorra 17%" copy on the frontend's yearly toggle. All prices are in soles (PEN).
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

        var canonicalNames = defaultPlans.Select(plan => plan.Name).ToHashSet();

        // Remove any plan that is not part of the canonical catalog so the "select plan" screen
        // never shows duplicates (e.g. "Plan Basico Mensual" at S/49.90 next to "Básico Mensual" at S/49).
        try
        {
            var existingPlans = await planRepository.ListAsync(cancellationToken);
            var duplicatePlans = existingPlans.Where(plan => !canonicalNames.Contains(plan.Name)).ToList();
            foreach (var plan in duplicatePlans)
            {
                planRepository.Remove(plan);
                logger.LogInformation("Removing non-canonical subscription plan '{Name}' (S/{Price}).", plan.Name, plan.Price);
            }

            if (duplicatePlans.Count > 0)
                await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to prune non-canonical subscription plans.");
        }

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
