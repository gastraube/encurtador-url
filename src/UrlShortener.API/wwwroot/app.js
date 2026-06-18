const API_BASE = "http://localhost:5210";

document.getElementById("shortenBtn").addEventListener("click", shortenUrl);
document.getElementById("copyBtn").addEventListener("click", copyToClipboard);

async function shortenUrl() {
  const originalUrl = document.getElementById("originalUrl").value;
  const customAlias = document.getElementById("customAlias").value;

  if (!originalUrl) {
    showError("Por favor, insira uma URL válida.");
    return;
  }

  try {
    const response = await fetch(`${API_BASE}/api/url/shorten`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        originalUrl: originalUrl,
        customAlias: customAlias || null,
      }),
    });

    if (!response.ok) {
      const error = await response.json();
      showError(error.message || "Erro ao encurtar URL");
      return;
    }

    const data = await response.json();
    showSuccess(data);
    hideError();
  } catch (error) {
    showError(`Erro: ${error.message}`);
  }
}

function showError(message) {
  const errorDiv = document.getElementById("errorMessage");
  errorDiv.textContent = message;
  errorDiv.style.display = "block";
  document.getElementById("successMessage").style.display = "none";
}

function hideError() {
  document.getElementById("errorMessage").style.display = "none";
}

function showSuccess(data) {
  document.getElementById("shortenedUrlInput").value =
    `${API_BASE}${data.shortenedUrl}`;
  document.getElementById("originalLink").href = data.originalUrl;
  document.getElementById("originalLink").textContent = data.originalUrl;
  document.getElementById("accessCount").textContent = data.accessCount;
  document.getElementById("successMessage").style.display = "block";
}

function copyToClipboard() {
  const input = document.getElementById("shortenedUrlInput");
  input.select();
  document.execCommand("copy");
  alert("Copiado para a área de transferência!");
}
