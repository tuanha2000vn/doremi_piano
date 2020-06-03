var MIDIUnifiedPlugin = {
    $G__postset: 'G.init();',
    $G: {
        initialized: false,
        midi: null,
        data: new Array(),
        ins : new Array(),
        outs : new Array(),
        lastId : 0,

        init: function()
        {
            console.log("MIDIUnified WEBGL : init()");
            
            if (navigator.requestMIDIAccess) {
                
                navigator.requestMIDIAccess({
                    sysex: false,
                    software: true
                }).then(G.onMIDISuccess, G.onMIDIFailure);

                console.log("MIDIUnified WEBGL : requestMIDIAccess supported");

            } else {

                console.log("MIDIUnified WEBGL : requestMIDIAccess NOT supported");

            }
        },

        getUniqueId: function(){
            G.lastId++;
            return G.lastId;
        },

        onMIDISuccess: function(midiAccess) {
            G.midi = midiAccess;
            G.initialized = true;
                        
            //var inputs = G.midi.inputs.values();
            //for (var input = inputs.next(); input && !input.done; input = inputs.next()) {
            //  input.value.onmidimessage = G.onMIDIMessage;
            //}

            console.log("MIDIUnified WEBGL : initialized");
        },

        onMIDIFailure: function(error) {
            console.log("MIDIUnified WEGL : Failed to initialize");
            G.initialized = false;
        },

        onMIDIMessage: function(message) {
            //console.log("MIDIUnified WEGL : " + message.data);
            G.data.push(message.data);
        }   
    },

    MidiIn_Initialized: function()
    {
        //console.log("MIDIUnified WEBGL : MidiIn_Initialized");
        return G.initialized && G.midi !== undefined;
    },

    MidiIn_PortOpen: function(i)
    {
        if(G.initialized && G.midi !== undefined && G.midi.inputs != undefined){
            var k = 0;
            var id = -1;

            var inputs = G.midi.inputs.values();
            for (var input = inputs.next(); input && !input.done; input = inputs.next()) {
                for(var j = 0; j<G.ins; j++){
                    if(G.ins[j][1] == port.id){
                        console.log("MIDIUnified WEGL : MidiIn_PortOpen Already Opened " + id + " " + input.value.id);
                        return G.ins[j][0];
                    }
                }

                if(k == i){
                    id = G.getUniqueId();
                    console.log("MIDIUnified WEGL : MidiIn_PortOpen " + id + " " + input.value.id);
                    input.value.onmidimessage = G.onMIDIMessage;
                    G.ins.push([id, input.value.id]);
                    return id;
                }
                k++;
            }

            return id;
        }  else {
            return -1;       
        }
    },

    MidiIn_PortClose: function(id)
    {
       console.log("MIDIUnified WEGL : MidiIn_PortClose Closing " + id);
       if(G.initialized && G.midi !== undefined && G.midi.inputs != undefined){
            
            var index = -1;

            G.midi.inputs.forEach( function( port, key ) {
                for(var i = G.ins.length - 1; i >=0;i--){
                    if(G.ins[i][0] == id){
                        if(G.ins[i][1] == port.id){
                            console.log("MIDIUnified WEGL : MidiIn_PortClose Closed " + port.id);
                            port.close();
                            index = i;
                        }
                    }
                }
            });

            if(index >= 0){
                G.ins.splice(index, 1);            
            }
        } 
    },

    MidiIn_PortCloseAll: function()
    {
       if(G.initialized && G.midi !== undefined && G.midi.inputs != undefined){
            G.midi.inputs.forEach( function( port, key ) {
                console.log("MIDIUnified WEGL : MidiIn_PortCloseAll " + port.id);
                port.close();
            });

            G.ins = new Arrya();
        } 
    },

    MidiIn_PortName: function(i)
    {
        var result = "";

        if(G.initialized && G.midi !== undefined && G.midi.inputs != undefined){
            var k = 0;
            G.midi.inputs.forEach( function( port, key ) {
                if(k == i){
                    result = port.name;
                }
                k++;
            });
        } 
        
        var buffer = _malloc(lengthBytesUTF8(result) + 1);
        writeStringToMemory(result, buffer);
        return buffer;
    },

    MidiIn_PortCount: function()
    {
        if(G.initialized && G.midi !== undefined){

            return G.midi.inputs.size;

        } else {

            return 0;        

        }
    },

    MidiIn_PortAdded: function()
    {
        if(G.initialized && G.midi !== undefined){

            return false;

        } else { 

            return false;

        }
    },

    MidiIn_PortRemoved: function()
    {
        if(G.initialized && G.midi !== undefined){

            return false;

        } else { 

            return false;

        }
    },

    MidiIn_PopMessage: function(array, size)
    {
        if(G.initialized && G.midi !== undefined && G.data !== undefined){
            
            if(G.data.length > 0){
                
                var data = G.data.shift();
                
                var i = 0;
                
                while(data !== undefined && data.length > 0 && i < data.length){
                    HEAPU8[array + i] = data[i];
                    i++;
                }

                return i;

            } else {
                
                return 0;            

            }
        } else {

            return 0;       

        } 
    },

    MidiOut_Initialized: function()
    {
        //console.log("MIDIUnified WEBGL : MidiOut_Initialized");
        return G.initialized && G.midi !== undefined;
    },

    MidiOut_PortOpen: function(i)
    {
        if(G.initialized && G.midi !== undefined && G.midi.outputs != undefined){
            var k = 0;
            var id = -1;

            var outputs = G.midi.outputs.values();
            for (var output = outputs.next(); output && !output.done; output = outputs.next()) {
                for(var j = 0; j<G.outs; j++){
                    if(G.outs[j][1] == port.id){
                        console.log("MIDIUnified WEGL : MidiOut_PortOpen Already Opened " + id + " " + output.value.id);
                        return G.outs[j][0];
                    }
                }

                if(k == i){
                    id = G.getUniqueId();
                    console.log("MIDIUnified WEGL : MidiOut_PortOpen " + id + " " + output.value.id);
                    G.outs.push([id, output.value.id]);
                    return id;
                }
                k++;
            }

            return id;
        }  else {

            return -1;       

        }
    },

    MidiOut_PortClose: function(id)
    {
        console.log("MIDIUnified WEGL : MidiOut_PortClose Closing " + id);
        if(G.initialized && G.midi !== undefined && G.midi.outputs != undefined){
            
            var index = -1;

            G.midi.outputs.forEach( function( port, key ) {
                for(var i = G.outs.length - 1; i >=0;i--){
                    if(G.outs[i][0] == id){
                        if(G.outs[i][1] == port.id){
                            console.log("MIDIUnified WEGL : MidiOut_PortClose Closed " + port.id);
                            port.close();
                            index = i;
                        }
                    }
                }
            });
            
            if(index >= 0){
                 G.outs.splice(index, 1);
            }
        } 
    },

    MidiOut_PortCloseAll: function()
    {
        if(G.initialized && G.midi !== undefined && G.midi.outputs != undefined){
            G.midi.outputs.forEach( function( port, key ) {
                console.log("MIDIUnified WEGL : MidiOut_PortCloseAll " + port.id);
                port.close();
            });

            G.outs = new Array();
        } 
    },

    MidiOut_PortName: function(i)
    {
        var result = "";

        if(G.initialized && G.midi !== undefined && G.midi.outputs != undefined){
            var k = 0;
            G.midi.outputs.forEach( function( port, key ) {
                if(k == i){
                    result = port.name;
                }
                k++;
            });
        } 
        
        var buffer = _malloc(lengthBytesUTF8(result) + 1);
        writeStringToMemory(result, buffer);
        return buffer;
    },

    MidiOut_PortCount: function()
    {
        if(G.initialized && G.midi !== undefined){
            return G.midi.outputs.size;
        } else {
            return 0;        
        }
    },

    MidiOut_PortAdded: function()
    {
        if(G.initialized && G.midi !== undefined){
            return false;
        } else { 
            return false;
        }
    },

    MidiOut_PortRemoved: function()
    {
        if(G.initialized && G.midi !== undefined){
            return false;
        } else { 
            return false;
        }
    },
        
    MidiOut_SendMessage: function(command, data1, data2)
    {
        return 0;
    },   

    MidiOut_Send: function(data, dataSize)
    {
        return 0;
    }   
};

autoAddDeps(MIDIUnifiedPlugin, '$G');
mergeInto(LibraryManager.library, MIDIUnifiedPlugin);