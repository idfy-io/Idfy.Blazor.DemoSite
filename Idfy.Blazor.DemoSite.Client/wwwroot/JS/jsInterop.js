window.blazorExtras = {

    setLocalStorage: (key, value) => {
        window.localStorage.setItem(key, value);
    },

    getLocalStorage: (key) => {
        return window.localStorage.getItem(key);
    },

    clearLocalStorage: (key) => {
        window.localStorage.clear();
    },

    readUploadedFileAsText: (fileInputId) => {

        return new Promise((resolve, reject) => {
            const file = document.getElementById(fileInputId).files[0];
            if (file) {
                var reader = new FileReader();
                reader.onerror = () => {
                    reader.abort();
                    reject(new DOMException("Problem parsing input file."));
                };
                reader.onload = function (evt) {
                    var data = {
                        content: evt.target.result,
                        name: file.name
                    };
                    resolve(data);
                };
                reader.readAsDataURL(file);
            };
        });
    },

    openUrlExternal: (url) => {
        // Electron
        var isElectron = window && window.process && window.process.type;
        if (isElectron) {
            let shell = require('electron').shell
            shell.openExternal(url);
        } else {
            window.open(url, "_blank");
        }
    },

    addMessageEventListener: (instance) => {
        window.addEventListener("message", receiveMessage, false);

        function receiveMessage(event) {           
            instance.invokeMethodAsync('GetWebMessage', event.data);            
        }       
    }

};

