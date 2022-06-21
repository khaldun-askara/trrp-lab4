import React from "react";

const Message = (props) => (
    <div className="message">
        <header className="message__header">{props.sender}</header>
        <p className="message__text">{props.message}</p>
    </div>
)

export default Message;