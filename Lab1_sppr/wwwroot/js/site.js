document.addEventListener("DOMContentLoaded", function () {
    document.addEventListener("click", function (event) {
        const target = event.target;

        if (target.classList.contains("page-link")) {
            event.preventDefault();

            const url = target.getAttribute("href");

            fetch(url, {
                method: "GET",
                headers: {
                    "X-Requested-With": "XMLHttpRequest"
                }
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error("Network response was not ok");
                    }
                    return response.text();
                })
                .then(html => {
                    const targetContainer = document.getElementById("product-list");
                    if (targetContainer) {
                        targetContainer.innerHTML = html;
                    }
                })
                .catch(error => {
                    console.error("Fetch error:", error);
                });
        }
    });
});
