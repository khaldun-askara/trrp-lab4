import React, { useState, useRef, useEffect, useCallback } from "react";
import Message from "./components/message";
import Form from "./components/form";
import Croc_canvas from "./components/croc_canvas";
import Suggestions from "./components/suggestions";

const client = new WebSocket('ws://127.0.0.1');

class App extends React.Component {
  componentWillMount() {
    client.onopen = () => {
      console.log('WebSocket Client Connected');
      client.send("WebSocket rocks");
    };
    client.onmessage = (message) => {
      console.log(message);
    };
    client.onerror = (e) => {
      //writeToScreen("<span class=error>ERROR:</span> " + e.data);
  };
  }

  sendMessage = async (e) => {
    e.preventDefault();
    const message = e.target.elements.message.value;
    client.send(message);
    console.log(message);
  }
  
  render() {
    return (
      <div className="wrapper">
        <Croc_canvas />
        <div>
          <div className="chat">
            <Message />
            <Message />
            <Message />
            <Message />
            <Message />
            <Message />
          </div>
          <Form sendMessage={this.sendMessage}/>
          <Suggestions />
        </div>
      </div>
    );
  }
}

export default App