document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('uploadForm').addEventListener('submit', function (e) {
        e.preventDefault();

        // Get the values of the input fields
        const path1 = document.getElementById('pdf1path').value;
        const path2 = document.getElementById('pdf2path').value;

        // Create an object with the paths
        const formData = {
            path1: path1,
            path2: path2
        };

        // Send the data to the API endpoint
        sendDataToAPI(formData);
    });
});


async function renderPDFs(differences) {
    const pdf1File = document.getElementById('pdf1').files[0];
    const pdf2File = document.getElementById('pdf2').files[0];

    const pdf1ArrayBuffer = await pdf1File.arrayBuffer();
    const pdf2ArrayBuffer = await pdf2File.arrayBuffer();

    const pdf1 = await getPDF(pdf1ArrayBuffer);
    const pdf2 = await getPDF(pdf2ArrayBuffer);

    renderPDF('pdfCanvas1', pdf1, differences);
    renderPDF('pdfCanvas2', pdf2, differences);
}


async function getPDF(arrayBuffer) {
    const loadingTask = pdfjsLib.getDocument({ data: arrayBuffer });
    return loadingTask.promise;
}




async function renderPDF(canvasId, pdfDoc, lineNumbers) {
    const page = await pdfDoc.getPage(1);
    const viewport = page.getViewport({ scale: 1.5 });
    const canvas = document.getElementById(canvasId);
    const context = canvas.getContext('2d');

    canvas.width = viewport.width;
    canvas.height = viewport.height;

    const renderContext = {
        canvasContext: context,
        viewport: viewport
    };

    await page.render(renderContext).promise;

    // Get the text content of the first page
    const textContent = await page.getTextContent();
    const textItems = textContent.items.filter(item => item.str.trim() !== '');
    console.log(textItems);

    const lineHeight = textItems[0].height; // Assuming all lines have the same height
    const lineSpacing = 5; // Adjust this value based on the spacing between lines
    context.fillStyle = 'rgba(117, 145, 184, 0.2)'; // color with transparency
    lineNumbers.forEach(lineNumber => {
        console.log("linenumber:", lineNumber);
        var lineIndex = lineNumber - 1; // Convert 1-based index to 0-based index
        if (lineIndex >= 0 && lineIndex < textItems.length) {
            const line = textItems[lineIndex];
            console.log(line);
            const lineY = viewport.height - (line.transform[5] * 1.5) - 11; // Adjusted y-coordinate
            context.fillRect(0, lineY, viewport.width, lineHeight + lineSpacing);
            console.log(lineY);
        }
    });
}

async function sendDataToAPI(formData) {

    await fetch('https://localhost:7052/getPDFComparison', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(formData)
    })
        .then(response => response.json())
        .then(data => {
            console.log(data);
            if (data.difference != null) {
                renderPDFs(data.difference);//render pdfs based on difference from backend
            }
        })
        .catch(error => {
            console.error('Error:', error);
        });
}

