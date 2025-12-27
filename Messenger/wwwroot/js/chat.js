class ChatClient {
    constructor(options) {
        this.recipientId = options.recipientId;
        this.myId = options.myId;
        this.messageInputId = options.messageInputId || '#messageInput';
        this.sendButtonId = options.sendButtonId || '#sendButton';
        this.messagesListId = options.messagesListId || '#messages-list';
        this.hubUrl = options.hubUrl || '/chatHub';

        this.connection = null;
        this.initialize();
    }

    initialize() {
        this.setupSignalR();
        this.setupEventHandlers();
    }

    setupSignalR() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(this.hubUrl)
            .configureLogging(signalR.LogLevel.Information)
            .build();

        this.connection.on("Receive", this.onMessageReceived.bind(this));

        this.connection.start()
            .then(() => {
                console.log("SignalR подключен");
                this.connection.invoke("JoinGroup", this.myId);
            })
            .catch(err => {
                console.error("Ошибка подключения SignalR:", err.toString());
            });

        this.connection.onclose(() => {
            console.log("Соединение закрыто");
            setTimeout(() => this.connection.start(), 5000);
        });
    }

    setupEventHandlers() {
        $(this.sendButtonId).click(this.sendMessage.bind(this));
        $(this.messageInputId).keypress(e => {
            if (e.which === 13) this.sendMessage();
        });
    }

    onMessageReceived(data) {
        const senderName = data.senderName || data.SenderName || "Неизвестный";
        const content = data.content || data.Content || data;

        $(this.messagesListId).append(
            `<div><strong>${senderName}:</strong> ${content}</div>`
        );

        this.scrollToBottom();
    }

    sendMessage() {
        const message = $(this.messageInputId).val().trim();
        if (!message) return;

        $.ajax({
            url: "/SendMessage",
            type: "POST",
            data: {
                recipientId: this.recipientId,
                content: message
            },
            success: () => $(this.messageInputId).val(""),
            error: (xhr, status, error) => {
                console.error("Ошибка отправки:", error);
            }
        });
    }

    scrollToBottom() {
        const container = $(this.messagesListId);
        container.scrollTop(container[0].scrollHeight);
    }

    disconnect() {
        if (this.connection) {
            this.connection.stop();
        }
    }
}

if (typeof window !== 'undefined') {
    window.ChatClient = ChatClient;
}