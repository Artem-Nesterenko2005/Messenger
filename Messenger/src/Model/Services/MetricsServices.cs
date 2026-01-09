using Prometheus;

namespace Messenger;

public interface IMetricsService
{
    void MessageSent(string fromUser, string toUser, string messageType);
    void MessageReceived(string user);
    void UserConnected(string userId);
    void UserDisconnected(string userId);
    void ConnectionError(string errorType);
    void MessageDeliveryTime(long milliseconds, string messageType);
}

public class MetricsService : IMetricsService
{
    private static readonly Counter _messagesSentCounter = Metrics
        .CreateCounter("messenger_messages_sent_total",
            "Total messages sent",
            labelNames: new[] { "from_user", "to_user", "content" });

    private static readonly Counter _messagesReceivedCounter = Metrics
        .CreateCounter("messenger_messages_received_total",
            "Total messages received",
            labelNames: new[] { "user" });

    private static readonly Counter _connectionsCounter = Metrics
        .CreateCounter("messenger_connections_total",
            "Total connections",
            labelNames: new[] { "user_id", "action" });

    private static readonly Counter _errorsCounter = Metrics
        .CreateCounter("messenger_errors_total",
            "Total errors",
            labelNames: new[] { "type" });

    private static readonly Gauge _activeConnectionsGauge = Metrics
        .CreateGauge("messenger_active_connections",
            "Currently active connections");

    private static readonly Histogram _messageDeliveryHistogram = Metrics
        .CreateHistogram("messenger_message_delivery_ms",
            "Message delivery time in milliseconds",
            new HistogramConfiguration
            {
                Buckets = new[] { 10.0, 50.0, 100.0, 200.0, 500.0, 1000.0, 2000.0 },
                LabelNames = new[] { "message_type" }
            });

    public void MessageSent(string fromUser, string toUser, string content)
    {
        _messagesSentCounter
            .WithLabels(fromUser ?? "system", toUser ?? "unknown", content)
            .Inc();
    }

    public void MessageReceived(string user)
    {
        _messagesReceivedCounter
            .WithLabels(user ?? "unknown")
            .Inc();
    }

    public void UserConnected(string userId)
    {
        _connectionsCounter
            .WithLabels(userId ?? "anonymous", "connect")
            .Inc();

        _activeConnectionsGauge.Inc();
    }

    public void UserDisconnected(string userId)
    {
        _connectionsCounter
            .WithLabels(userId ?? "anonymous", "disconnect")
            .Inc();

        _activeConnectionsGauge.Dec();
    }

    public void ConnectionError(string errorType)
    {
        _errorsCounter
            .WithLabels(errorType ?? "unknown")
            .Inc();
    }

    public void MessageDeliveryTime(long milliseconds, string messageType)
    {
        _messageDeliveryHistogram
            .WithLabels(messageType ?? "text")
            .Observe(milliseconds);
    }
}
