window.blazorExtras = {
    readUploadedFileAsText: function (fileInputId) {

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
    openUrlExternal: function (url) {
        // Electron
        var isElectron = window && window.process && window.process.type;
        if (isElectron) {
            let shell = require('electron').shell
            shell.openExternal(url);
        } else {
            window.open(url, "_blank");
        }
    }
};

