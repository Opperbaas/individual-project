const activityRows = document.getElementById("activityRows");
const addActivityRow = document.getElementById("addActivityRow");

function reindexActivityRows() {
    const rows = activityRows?.querySelectorAll(".activity-row") ?? [];

    rows.forEach((row, index) => {
        row.querySelectorAll("[name]").forEach(field => {
            field.name = field.name.replace(
                /ActivityRows\[\d+\]/,
                `ActivityRows[${index}]`);
        });

        row.querySelectorAll("[id]").forEach(field => {
            field.id = field.id.replace(
                /_\d+__/,
                `_${index}__`);
        });
    });
}

function filterRowLocations(row) {
    const typeSelect = row.querySelector(".row-location-type");
    const locationSelect = row.querySelector(".row-location");
    if (!typeSelect || !locationSelect) {
        return;
    }

    let firstVisible = null;
    for (const option of locationSelect.options) {
        const matches =
            option.dataset.locationType === typeSelect.value;
        option.hidden = !matches;

        if (matches && firstVisible === null) {
            firstVisible = option;
        }
    }

    if (firstVisible !== null &&
        (locationSelect.selectedOptions.length === 0 ||
         locationSelect.selectedOptions[0].hidden)) {
        locationSelect.value = firstVisible.value;
    }
}

function attachRowEvents(row) {
    const typeSelect = row.querySelector(".row-location-type");
    typeSelect?.addEventListener("change", () => filterRowLocations(row));

    row.querySelector(".remove-row")?.addEventListener("click", () => {
        const rows = activityRows.querySelectorAll(".activity-row");
        if (rows.length > 1) {
            row.remove();
            reindexActivityRows();
        }
    });

    filterRowLocations(row);
}

activityRows?.querySelectorAll(".activity-row").forEach(attachRowEvents);

addActivityRow?.addEventListener("click", () => {
    const firstRow = activityRows.querySelector(".activity-row");
    if (!firstRow) {
        return;
    }

    const newRow = firstRow.cloneNode(true);
    activityRows.appendChild(newRow);
    reindexActivityRows();
    attachRowEvents(newRow);
});
