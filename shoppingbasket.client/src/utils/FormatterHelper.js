export const formatDate = (d) => {
    if (!d) return "";
    const dt = new Date(d);
    return dt.toLocaleString();
};

export const formatCurrency = (v) =>
    typeof v === "number"
        ? v.toLocaleString(undefined, { style: "currency", currency: "EUR" })
        : "-";
