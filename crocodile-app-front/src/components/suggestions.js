import React from "react";
import Form from "./form";

const Suggestions = (props) => (
    <div className="suggestions">
        <p className="send-word-button">Предложить слово</p>
        <form className="form" onSubmit={props.sendSuggestion}>
            <input type="text" name="message" className="text-area" />
            <button className="send-button">Отправить</button>
        </form>
    </div>
)

export default Suggestions;