/*var start = function ()
{
	var inc = document.getElementById('incomming');
	var wsImpl = window.WebSocket || window.MozWebSocket;
	var form = document.getElementById('sendForm');
	var input = document.getElementById('sendText');
	
	inc.innerHTML += "connecting to server ..<br/>";

	// create a new websocket and connect
	window.ws = new wsImpl('ws://localhost:8181/consoleappsample', 'my-protocol');

	// when data is comming from the server, this metod is called
	ws.onmessage = function (evt) {
		inc.innerHTML += evt.data + '<br/>';
	};

	// when the connection is established, this method is called
	ws.onopen = function () {
		inc.innerHTML += '.. connection open<br/>';
	};

	// when the connection is closed, this method is called
	ws.onclose = function () {
		inc.innerHTML += '.. connection closed<br/>';
	}
	
	form.addEventListener('submit', function(e){
		e.preventDefault();
		var val = input.value;
		ws.send(val);
		input.value = "";
	});
	
}

window.onload = start;*/

(function()
{
	WS3Vglobal =
	{

	};
	
	function WS3VWebSocket(options)
	{
		if (!this instanceof WS3VWebSocket)
			return new WS3VWebSocket(options);

		else
		{
			if (!options)
				options = {};
	
			this.settings = MergeDefaults(this._defaultOptions, options);
	
			if (!window.WebSocket)
				throw 'UNSUPPORTED: Websockets are not supported in this browser!';
	
			this.SocketState = WS3VWebSocket.prototype.SocketStates.Closed;
			
			this.Connected = this.settings.Connected;
			this.Disconnected = this.settings.Disconnected;
		}
	}

	function WS3VWebSocket_send(props)
	{
		// http://ws3v.org/spec.json#send
		this.type = 5;
		
		props = props || {};
		this.id = props.id || "WS3V" + Math.floor(Math.random()*1001);
		this.method = props.method || '';
		this.uri = props.uri || '';
		this.parameters = props.parameters || null;
		this.callback = props.callback || function(data){};
		this.error = props.error || function(error, description, url){};
	}
	
	WS3VWebSocket_send.prototype.ToString = function()
	{
		if(this.parameters == null || typeof this.parameters == 'undefined' || this.parameters == '' && Object.keys(this.parameters).length === 0)
			return [this.type, this.id.toString(), this.method, this.uri];
				
		else
			return [this.type, this.id.toString(), this.method, this.uri, this.parameters];
	};
	
	function WS3VWebSocket_publish(props)
	{
		// http://ws3v.org/spec.json#publish
		this.type = 15;
		
		props = props || {};
		this.uri = props.uri || '';
		this.message = props.message || '';
		this.echo = props.echo || false;
	}
	
	WS3VWebSocket_publish.prototype.ToString = function()
	{
		if(this.echo)
			return [this.type, this.uri, this.message, this.echo];
				
		else
			return [this.type, this.uri, this.message];
	};
	
	function WS3VWebSocket_channels(props)
	{
		// http://ws3v.org/spec.json#channels
		this.type = 8;
		
		props = props || {};
		this.meta = props.meta || false;
		this.filter = props.filter || '';
		this.callback = props.callback || function(channels, meta){};
	}
	
	WS3VWebSocket_channels.prototype.ToString = function()
	{		
		if(this.filter == null || typeof this.filter == 'undefined' || this.filter == '')
		{
			if(!this.meta)
				return [this.type];
				
			else
				return [this.type, this.meta];
		}	
		else
			return [this.type, this.meta, this.filter];
	};
	
	function WS3VWebSocket_subscription(props)
	{
		props = props || {};
		this.uri = (props.uri == null || typeof props.uri == 'undefined' || props.uri == '') ? '' : props.uri;
		this.filter = (props.filter == null || typeof props.filter == 'undefined' || props.filter == '') ? '' : props.filter;
		this.connected = (props.connected == null || typeof props.connected != 'function') ? function(data){} : props.connected;
		this.events = (props.events == null || typeof props.events != 'function') ? function(data){} : props.events;
		this.deny = (props.deny == null || typeof props.deny != 'function') ? function(error, description, url){} : props.deny;
	}
	
	function WS3VWebSocket_subscribe(props)
	{
		// http://ws3v.org/spec.json#subscribe
		this.type = 10;
		
		props = props || {};
		this.uri = props.uri || [];
		this.filter = props.filter || [];
		this.connected = props.connected || [];
		this.events = props.events || [];
		this.deny = props.deny || [];
	}
	
	WS3VWebSocket_subscribe.prototype.ToString = function()
	{		
		if(this.filter == null || typeof this.filter == 'undefined' || this.filter.length <= 0)
			return [this.type, this.uri];
		else
			return [this.type, this.uri, this.filter];
	};
	
	function WS3VWebSocket_event(data)
	{
		// http://ws3v.org/spec.json#event
		
		data = data || {};
		this.message = data[2] || '';
		this.timestamp = data[3] || 0;
		this.timestamp = new Date((this.timestamp + 1308823200)*1000);
	}

  WS3VWebSocket.prototype =
  {
	  protocol_version: 1,
    _socket: {},
    latency: -1,
    settings: {},

    SocketStates:
	{
      Connecting: 0,
      Open: 1,
      Closing: 2,
      Closed: 3
    },

    SocketState: 3,
	
	authenticated: false,
	channels: false,
	channel_callback: function() { },
	
	session_id: '',
	
	message_index: 0,
	closed: false,
	
	message_queue: [],
	subscriptions: [],
	
	heart:
	{
		beat:-1,
		busy:false,
		lub:null,
		pacemaker:null
	},

    Connect: function()
	{
      var server = 'ws://' + this.settings.Server + ':' + this.settings.Port + '/' + this.settings.Action;
      var that = this;
      this._socket = new WebSocket(server);
      this._socket.onopen = function() { that._OnOpen(); };
      this._socket.onmessage = function(data) { that._OnMessage(data); };
      this._socket.onclose = function() { that._OnClose(); };
      this.SocketState = WS3VWebSocket.prototype.SocketStates.Connecting;

		if (this.settings.DebugMode)
		{
			if(!window.vkbeautify)
			{
		  		throw "DEBUG: Can't debug without asset vkbeautify.js, debug disabled!";
				this.settings.DebugMode = false;
			}
			else
			{
				// console
				var debugconsole = document.createElement('div');
				debugconsole.setAttribute("id", "debugconsole");
				document.getElementsByTagName('body')[0].appendChild(debugconsole);
				
				// client-side
				var debugclient = document.createElement('div');
				debugclient.setAttribute("id", "client");
				document.getElementById('debugconsole').appendChild(debugclient);
				
				// server-side
				var debugserver = document.createElement('div');
				debugserver.setAttribute("id", "server");
				document.getElementById('debugconsole').appendChild(debugserver);
				
				this.Debug("connecting to " + server, 'client');
			}
		}
    },
	
	Channels: function(props)
	{
		if(this.closed)
			return;

		if(!this.channels)
		{
			throw "ERROR: Channel listing on this server is not supported!";
			return;
		}

		var message = new WS3VWebSocket_channels(props);
		this.channel_callback = message.callback;
		this._Send(message.ToString());
	},
	
	Subscribe: function(props)
	{
		// make sure websocket is still open
		if(this.closed)
			return;

		// create the subscribe message object
		var message = new WS3VWebSocket_subscribe(props);
		
		// subscribe is unique as it takes an array of subscriptions
		for (var i = 0, len = message.uri.length; i < len; i++)
			// this builds the unique subscription object for callback tracking
			this.subscriptions.push(new WS3VWebSocket_subscription({uri:message.uri[i],filter:message.filter[i],connected:message.connected[i],events:message.events[i],deny:message.deny[i]}));
			
		// send the subscribe command to the server
		this._Send(message.ToString());
	},
	
	Publish: function(props)
	{
		// make sure websocket is still open
		if(this.closed)
			return;
			
		// todo, check if subscribed to channel, if not issue subscription command
		// and in connected callback fire the publish message
		// the problem would be where to hook event callback so this might not work

		// create the publish message object
		var message = new WS3VWebSocket_publish(props);
					
		// send the publish command to the server
		this._Send(message.ToString());
	},
	
	Send: function(props)
	{
		if(this.closed)
			return;
			
		props.id = ++this.message_index;
		var message = new WS3VWebSocket_send(props);
		this.message_queue.push(message);
		this._Send(message.ToString());
	},

	_Send: function(data)
	{
		if(this.closed)
			return;
			
		if (typeof data === 'object')
		
			data = JSON.stringify(data);
	
		this._socket.send(data);
	
		// we need to check and remove the heartbeat timeouts
		if(this.heart.beat != -1 && !this.heart.busy)
		{
			clearTimeout(this.heart.pacemaker);
	
			var that = this;
			this.heart.pacemaker = 	setTimeout(function()
			{
				that.heart.lub = (new Date()).getTime();
				that._Send("lub");
			}, that.heart.beat);
		}
	
		if (this.settings.DebugMode)
		
			this.Debug((data != 'lub') ? vkbeautify.json(data) : 'lub <3', 'client');
	},

	Stop: function()
	{
		this._socket.close();
	
		if (this.settings.DebugMode)
		  this.Debug('Closed connection.', 'client');
	},

    Connected: function() { },
    Disconnected: function() { },

    _OnOpen: function()
	{
      var instance = this;
      this.SocketState = WS3VWebSocket.prototype.SocketStates.Open;

      if (this.settings.DebugMode)
		  this.Debug('Connected.', 'client');
    },

    _OnMessage: function(event)
	{

      var that = this;

		// check for heartbeat message
		if(event.data == 'dub')
		{
			// this should always be true but just in case
			if(this.heart.beat != -1)
			{
				this.latency = (((new Date()).getTime()) - this.heart.lub);
				
				// if we can't always be checking the tunnel pulse then
				// we need to make sure we are within the interval
				// otherwise the interval pacemaker should already be alive and kicking
				if(!this.heart.busy)
				{
					// pump the pacemaker with the heartbeat interval
					this.heart.pacemaker = 	setTimeout(function()
					{
						that.heart.lub = (new Date()).getTime();
						that._Send("lub");
					}, that.heart.beat);
				}
				
				// show debug output
				if (this.settings.DebugMode)
				{
		  			this.Debug('dub <3', 'server');
					this.Debug('[ws3v diagnostics -> connection latency: ' + this.latency + 'ms]', 'client');
				}
			}
			
			// no need to go any further
			return;
		}

      if (this.settings.DebugMode)
		  this.Debug(vkbeautify.json(event.data), 'server');
		
		// extract the data and parse the JSON
		var data = JSON.parse(event.data);
		
	
		// if we are not authenticated then either we need to respond to a gatekeeper
		// or the message could be the howdy which means we are authorized
		
		if(!this.authenticated)
		{
			// this is a gatekeeper request, respond with signature
			if(data[0] == 1)
			{
				// generate signature and send
				var response = WS3VWebSocket.prototype.signature;
				response.credentials = this.settings.Credentials;
				this._Send(response.ToString());
			}
			
			// this message is a howdy, meaning the server is ok talking to us
			// without any sort of authentication
			else if(data[0] == 3)
			{
				// validate server protocol versions match
				if(this.protocol_version != data[2])
				{
					// crap, they dont match, trigger connection close and throw the error
					this._OnClose();
					throw 'UNSUPPORTED: WS3V Protocol Version Mismatch, Client: ' + this.protocol_version + ', Server: ' + data[2];
					return;
				}
				
				// process howdy message
				// get session id information
				this.session_id = data[1];
				
				// channel listing
				if(data[7] === true)
					this.channels = true;
				
				// check if heartbeats are enabled
				if(data[4][0] >= 0)
				{
					// calculate the heartbeat interval
					// we are using the average between the low and high
					// this is very conservative calculation and can be tweeked
					this.heart.beat = Math.round(Math.abs(data[4][1] - data[4][0]) / 2) * 1000;
					this.heart.busy = data[4][2];
					
					// seems the server doesnt care if we heartbeat at a steady interval
					// to collect latency diagnostic data.. so lets do it!
					if(this.heart.busy)
					{	
						// pump the pacemaker with the first beat at steady interval
						this.heart.pacemaker = 	setInterval(function()
						{
							that.heart.lub = (new Date()).getTime();
							that._Send("lub");
						}, that.heart.beat);
					}
					
					// we need to setup the timeout mode
					else
					{
						// pump the pacemaker with first beat and schedual
						this.heart.pacemaker = 	setTimeout(function()
						{
							that.heart.lub = (new Date()).getTime();
							that._Send("lub");
						}, that.heart.beat);
					}
				}
				
				// now authenticated, call the connected callback
				this.authenticated = true;
				this.Connected();
			}
			
			// else.... this isnt good, the server might not speak WS3V
			// so its safe to abort the connection
			
			else
				this._OnClose();
			
			// if this is an aux message, meaning the client code needs to communicate directly with the
			// server, then we dont fire the call back since it never really happened
			// we should always return at this point
			return;
		}
		
		// so we are authenticated, lets see what the server sent us
		// http://ws3v.org/spec.json
		switch(data[0])
		{
			case 6:
			
				// this is a RPC response, lets find the message in the queue and fire the callback
				var message = RetrieveMessage(this.message_queue, data[1]);
						
				// make sure its not null
				if(typeof message[0] != "undefined")
				{
					if (message[0].callback instanceof Function)
						message[0].callback(data[2]);
				}
				
				break;
			  
			 case 7:
			 
				// this is a RPC error response, lets find the message in the queue and fire the error callback
				var message = RetrieveMessage(this.message_queue, data[1]);
						
				// make sure its not null
				if(typeof message[0] != "undefined")
				{
					if (message[0].error instanceof Function)
						message[0].error(data[2], data[3], data[4]);
				}
				
				break;
			  
			case 9:
			
				// this is a listings message response
				if (this.channel_callback instanceof Function)
					this.channel_callback(data[1], data[2]);

				break;
				
			case 11:
				// this is a acknowledge message response
				var subscription = GetSubscription(this.subscriptions, data[1], false);
					
				// make sure its not null
				if(typeof subscription != "undefined")
				{
					if (subscription.connected instanceof Function)
						subscription.connected(data);
				}

				break;
				
			case 16:
				// event from a subscription
				var subscription = GetSubscription(this.subscriptions, data[1], false);
					
				// make sure its not null
				if(typeof subscription != "undefined")
				{
					if (subscription.events instanceof Function)
						subscription.events(new WS3VWebSocket_event(data));
				}

				break;
			  
			default:
			  
		}
    },

	_OnClose: function()
	{
		// make sure we didnt close already
		if(this.closed)
			return;
		
		// set close flag
		this.closed = true;
		
		if (this.settings.DebugMode)
			this.Debug('Connection closed.', 'client');

		// set socket state and fire disconnect callback
		this.SocketState = WS3VWebSocket.prototype.SocketStates.Closed;
		this.Disconnected();
	  
		// we need to check and remove the heartbeat intervals
		if(this.heart.beat != -1)
		{
			// clear the timeout or interval
			if(this.heart.busy)
				clearInterval(this.heart.pacemaker);
				
			else
				clearTimeout(this.heart.pacemaker);
		}
    },
	
	// debug function, output pretty server and client logs
	Debug: function(message, location)
	{ 
		if (this.settings.DebugMode)
			document.getElementById(location).innerHTML = "<pre><code>" + message + "</code></pre>" + document.getElementById(location).innerHTML;
	}
  };

	// default options for the WS3V Websocket
	WS3VWebSocket.prototype._defaultOptions =
	{
		Port: 80,
		Server: '',
		Action: '',
		
		Credentials: [],
		
		Connected: function() { },
		Disconnected: function() { },
		
		DebugMode: false
	};
  
	WS3VWebSocket.prototype.signature =
	{
		type: 2,
		credentials: [],
	
		ToString: function()
		{
			return [this.type, this.credentials];	
		}
	};

	// used to itterate over the supplied instance and set defaults
	// basically a poormans constructor with a default object
	function MergeDefaults(o1, o2)
	{
		var o3 = {};
		var p = {};
	
		// load defaults
		for (p in o1)
			o3[p] = o1[p];
	
		// overwrite with provided settings
		for (p in o2)
			o3[p] = o2[p];
	
		return o3;
	}
  
	// little trick to remove a queued message by id and return the message for processing
	// used to simulate guranteed response in an async enviornment
  	function RetrieveMessage(array, id)
	{
    	for (var i = 0, len = array.length; i < len; i++)
		{
			if (array[i].id == id)
				return array.splice(i, 1);
    	}
    	return null;
	}
	
	// used to pull or remove a subscription based on uri
  	function GetSubscription(array, uri, remove)
	{
    	for (var i = 0, len = array.length; i < len; i++)
		{
			if (array[i].uri == uri)
			{
				if(remove)
					return array.splice(i, 1);
				else
					return array[i];
			}
    	}
    	return null;
	}

	// attach these objects to the window
	window.WS3VWebSocket = WS3VWebSocket;
	window.MergeDefaults = MergeDefaults;
	window.RetrieveMessage = RetrieveMessage;
	
	// oh firefox, way to pull an ie.. no wonder people don't like you anymore
	if(window.MozWebSocket)
		window.WebSocket = MozWebSocket;

})(window);