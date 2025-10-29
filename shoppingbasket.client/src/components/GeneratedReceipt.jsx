import "/src/styles/GeneratedReceipt.css";
import { formatDate, formatCurrency } from "../utils/FormatterHelper";

function GeneratedReceipt({ receipt, onClose }) {
  if (!receipt) return null;

  // defensive: normalize items array shape
  const itemsOrdered =
    receipt.itemsOrdered || receipt.itemsOrdered || receipt.ItemsOrdered || [];

  return (
    <section className="generated-receipt">
      <header className="receipt-header">
        <h2>Receipt</h2>
        <div>
          <div>
            <strong>Number:</strong>{" "}
            {receipt.receiptNumber || receipt.receiptId || ""}
          </div>
          <div>
            <strong>Created:</strong> {formatDate(receipt.createdDateTime)}
          </div>
          <div>
            <strong>Total:</strong> {formatCurrency(receipt.totalCost)}
          </div>
        </div>
        <button type="button" className="close-button" onClick={onClose}>
          Close
        </button>
      </header>

      <table
        className="receipt-table"
        role="table"
        aria-label="Generated receipt"
      >
        <thead>
          <tr>
            <th>Item Description</th>
            <th>Quantity</th>
            <th>Sub Total</th>
            <th>Discounted</th>
            <th>Line Total</th>
          </tr>
        </thead>
        <tbody>
          {itemsOrdered.length === 0 && (
            <tr>
              <td colSpan="5">No items</td>
            </tr>
          )}
          {itemsOrdered.map((it) => (
            <tr key={it.itemOrderedId || `${it.itemId}-${Math.random()}`}>
              <td>{it.itemDescription}</td>
              <td>{it.quantity}</td>
              <td>{formatCurrency(it.subTotalCost)}</td>
              <td>
                {it.discountedCost != null
                  ? formatCurrency(it.discountedCost)
                  : "-"}
              </td>
              <td>{formatCurrency(it.totalCost)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </section>
  );
}

export default GeneratedReceipt;
