import { dotnet } from './_framework/dotnet.js'

const { getAssemblyExports, getConfig, runMain } = await dotnet
    .withDiagnosticTracing(false)
    .create();

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);

dotnet.instance.Module['canvas'] = document.getElementById('canvas');

function mainLoop() {
    exports.RaylibWasm.Application.UpdateFrame();

    window.requestAnimationFrame(mainLoop);
}

export function offerFileAsDownload(filename) {
    let content = dotnet.instance.Module.FS.readFile(filename);
    console.log(`Offering download of "${filename}", with ${content.length} bytes...`);

    var a = document.createElement('a');
    a.download = filename;
    a.href = URL.createObjectURL(new Blob([content], {type: "application/octet-stream"}));
    a.style.display = 'none';

    document.body.appendChild(a);
    a.click();
    setTimeout(() => {
        document.body.removeChild(a);
        URL.revokeObjectURL(a.href);
    }, 2000);
}

await runMain();
window.requestAnimationFrame(mainLoop);