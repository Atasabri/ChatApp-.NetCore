"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();


connection.on("ReceiveMessage", function (user, message,type,chatid) {
    // var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    if (type == "Message") {
        var encodedMsg = user + " :: " + message;
        var li = document.createElement("li");
        li.textContent = encodedMsg;
        document.getElementById("messagesList").appendChild(li);
    } else if (type == "AddPrivate") {
        alert(message)
        $("#sidebar").append("<header><a href='/Home/Chat?ChatID=" + chatid + "'>" + user + "</a></header>");
    } else if (type == "AddGroup") {
        alert(message)
        $("#sidebar").append("<header><a href='/Home/Chat?ChatID=" + chatid + "'>" + user + "</a></header>");
    }
    
});

var chatid = null;
var chat = document.getElementById("ChatID");
if (chat != null) {
    chatid = chat.value;
}

connection.start().then(function () {
    if (chatid != null) {
        connection.invoke("JoinGroup", chatid).catch(function (err) {
            return console.error(err.toString());
        });
    }

}).catch(function (err) {
    return console.error(err.toString());
});

if (chatid != null) {
    document.getElementById("sendButton").addEventListener("click", function (event) {
        var message = document.getElementById("messageInput").value;
        $.post("/Home/SendMessage", {

            Text: message,
            ChatID: chatid

        },
            function (data) {
                console.log(data)
            }

        )
        event.preventDefault();
    });
}
