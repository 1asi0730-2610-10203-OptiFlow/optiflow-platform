using optiflow_platform.PatientCenter.Application.Services;
using optiflow_platform.PatientCenter.Domain.Model.Entities;
using optiflow_platform.PatientCenter.Domain.Model.Queries;
using optiflow_platform.PatientCenter.Domain.Repositories;

namespace optiflow_platform.PatientCenter.Application.Internal.QueryServices;

public class PatientNotificationQueryService(IPatientNotificationRepository notificationRepository)
    : IPatientNotificationQueryService
{

    public async Task<IEnumerable<PatientNotification>> Handle(GetNotificationsByPatientIdQuery query, CancellationToken cancellationToken)
    {
        var patientId = query.PatientId;

        var existingNotifications = await _notificationRepository.GetByPatientIdAsync(patientId, cancellationToken);

        var currentOrders = await _patientOrderQueryService.Handle(new GetOrdersByPatientIdQuery(patientId), cancellationToken);

        bool checkNewNotifications = false;

        foreach (var order in currentOrders)
        {
            var alreadyNotified = existingNotifications.Any(n => n.WorkOrderId == order.Id && n.Message.Contains(order.Status.ToString()));

            if (!alreadyNotified)
            {
                var createCommand = new CreateNotificationCommand(
                    patientId,
                    order.Id,
                    $"Tu pedido N° {order.OrderNumber} ha cambiado al estado: {order.Status}",
                    "PENDING"
                );
                
                await _notificationCommandService.Handle(createCommand, cancellationToken);
                checkNewNotifications = true;
            }
        }
        if (checkNewNotifications)
        {
            existingNotifications = await _notificationRepository.GetByPatientIdAsync(patientId, cancellationToken);
        }

        return existingNotifications;
    }
}