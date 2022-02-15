import React, { useState, useRef, useEffect, useCallback } from "react";

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
  
  render() {
    return (
      <div>
        <p>Practical Intro To WebSockets.</p>
        <button>go fuck yourself</button>
      </div>
    );
  }
}

export default App