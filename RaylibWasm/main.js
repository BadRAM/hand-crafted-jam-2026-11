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

// Disable some hotkeys while canvas clicked.
let clickedInCanvas = false
window.addEventListener("click", (ev) => {
    clickedInCanvas = ev.target.tagName.toUpperCase() == "CANVAS";
    update();
});
window.addEventListener(
    "keydown",
    (event) => {
        if (clickedInCanvas) {
            if (
                ["Space", "ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight"].indexOf(
                    event.code
                ) > -1
            ) {
                event.preventDefault();
            }
        }
        if (event.code == "Escape") {
            clickedInCanvas = false;
            update();
        }
    },
    false
);

// Game can call this to initiate a download
export function offerFileAsDownload(filename) {
    let content = dotnet.instance.Module.FS.readFile(filename);
    console.log(`Offering download of "${filename}", with ${content.length} bytes...`);

    var anchor = document.createElement('a');
    anchor.download = filename;
    anchor.href = URL.createObjectURL(new Blob([content], {type: "application/octet-stream"}));
    anchor.style.display = 'none';

    document.body.appendChild(anchor);
    anchor.click();
    setTimeout(() => {
        document.body.removeChild(anchor);
        URL.revokeObjectURL(anchor.href);
    }, 2000);
}

await runMain();
window.requestAnimationFrame(mainLoop);