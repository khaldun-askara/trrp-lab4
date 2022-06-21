import React from "react";

const Form = props => (
    <form className="form" onSubmit={props.sendMessage}>
        <input type="text" name="message" className="text-area" />
        <button className="send-button">Отправить</button>
    </form>
)

export default Form;