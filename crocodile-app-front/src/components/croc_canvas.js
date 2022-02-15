import React from "react";
import CanvasDraw from "react-canvas-draw";

const Croc_canvas = (props) => (
    <form className="drawing-space">
        <header className="message__header">Слово для рисования</header>
        <CanvasDraw brushRadius={5} />
        <button>Сдаться</button>
    </form>
)

export default Croc_canvas;