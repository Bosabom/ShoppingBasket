// import { StrictMode } from "react";
// import { createRoot } from "react-dom/client";
import "/src/styles/BasketForm.css";
import QuantityInput from "./UI/QuantityInput";

function BasketForm() {
  return (
    <form id="basket-form" onSubmit={handleSubmit}>
      <QuantityInput title="Soup (€0.65)" inputClassName="quantity-inputs" />
      <QuantityInput title="Bread (€0.8)" inputClassName="quantity-inputs" />
      <QuantityInput title="Milk (€1.3)" inputClassName="quantity-inputs" />
      <QuantityInput
        title="Apples (€1.00 per bag)"
        inputClassName="quantity-inputs"
      />
      <div className="clearfix"></div>
      <input type="submit" id="button-submit-form" value="Submit" />
    </form>
  );
}

function handleSubmit() {
  window.alert("FormIsSubmitted!");

  // createRoot(document.getElementById("receipt-root")).render(
  //   <StrictMode>
  //     <Receipt />
  //   </StrictMode>
  // );
}

export default BasketForm;
