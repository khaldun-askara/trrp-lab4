import React from "react";
import CanvasDraw from "react-canvas-draw";

const Croc_canvas = (props) => (
    <form className="drawing-space">
        <header>Рисующий: {props.role}</header>
        <header className="message__header">Слово: {props.word}</header>
        <CanvasDraw brushRadius={5} />
    </form>
)

export default Croc_canvas;