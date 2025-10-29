import "/src/styles/BasketForm.css";
import { useState } from "react";
import { useForm } from "react-hook-form";
import httpService from "../utils/httpService";
import GeneratedReceipt from "./GeneratedReceipt";

function BasketForm() {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const [receipt, setReceipt] = useState(null);
  const [error, setError] = useState(null);

  //TODO: move this to separate receiptsService based on httpService with defined requests
  const createReceipt = async (requestBody) => {
    try {
      setError(null);
      const response = await httpService.post("/receipts", requestBody);

      //handle response
      const created = response && response.data ? response.data : response;
      setReceipt(created);
    } catch (err) {
      //handle error
      console.error("Error creating receipt", err);
      setError(
        err && err.response.data.detail
          ? err.response.data.detail
          : "Failed to create receipt. See console for details."
      );
    }
  };

  //TODO: fetch items and discounts data from API to populate into form
  return (
    <>
      {/*Shopping basket form*/}
      <form id="basket-form" onSubmit={handleSubmit(createReceipt)}>
        {/* TODO: move inputs to separate component with this signatures and errors handling */}

        {/*Soup quantity input*/}
        <label htmlFor="soup-quantity">Soup (€0.65)</label>
        <input
          type="number"
          id="soup-quantity"
          defaultValue="0"
          className="quantity-inputs"
          {...register("soupQuantity", { required: true, min: 0 })}
          aria-invalid={errors.soupQuantity ? "true" : "false"}
        />
        {errors.soupQuantity?.type === "required" && (
          <p role="alert" className="error-message">
            Soup quantity is required!
          </p>
        )}
        {errors.soupQuantity?.type === "min" && (
          <p role="alert" className="error-message">
            Soup quantity should be greater than 0!
          </p>
        )}

        {/*Bread quantity input*/}
        <label htmlFor="bread-quantity">Bread (€0.8)</label>
        <p className="discount-description">
          Buy 2 tins of soup and get a loaf of bread for half price
        </p>
        <input
          type="number"
          id="bread-quantity"
          defaultValue="0"
          className="quantity-inputs"
          {...register("breadQuantity", { required: true, min: 0 })}
          aria-invalid={errors.breadQuantity ? "true" : "false"}
        />
        {errors.breadQuantity?.type === "required" && (
          <p role="alert" className="error-message">
            Bread quantity is required!
          </p>
        )}
        {errors.breadQuantity?.type === "min" && (
          <p role="alert" className="error-message">
            Bread quantity should be greater than 0!
          </p>
        )}

        {/*Milk quantity input*/}
        <label htmlFor="milk-quantity">Milk (€1.3)</label>
        <input
          type="number"
          id="milk-quantity"
          defaultValue="0"
          className="quantity-inputs"
          {...register("milkQuantity", { required: true, min: 0 })}
          aria-invalid={errors.milkQuantity ? "true" : "false"}
        />
        {errors.milkQuantity?.type === "required" && (
          <p role="alert" className="error-message">
            Milk quantity is required!
          </p>
        )}
        {errors.milkQuantity?.type === "min" && (
          <p role="alert" className="error-message">
            Milk quantity should be greater than 0!
          </p>
        )}

        {/*Apples quantity input*/}
        <label htmlFor="apples-quantity">Apples (€1.00 per bag)</label>
        <p className="discount-description">10% off this week</p>
        <input
          type="number"
          id="apples-quantity"
          defaultValue="0"
          className="quantity-inputs"
          {...register("applesQuantity", { required: true, min: 0 })}
          aria-invalid={errors.applesQuantity ? "true" : "false"}
        />
        {errors.applesQuantity?.type === "required" && (
          <p role="alert" className="error-message">
            Apples quantity is required!
          </p>
        )}
        {errors.applesQuantity?.type === "min" && (
          <p role="alert" className="error-message">
            Apples quantity should be greater than 0!
          </p>
        )}
        <input type="submit" id="button-submit-form" value="Submit" />
      </form>

      {error && <p className="error-message">{error}</p>}

      {/* Render generated receipt when available */}
      {receipt && (
        <div className="generated-receipt-wrapper">
          <GeneratedReceipt
            receipt={receipt}
            onClose={() => setReceipt(null)}
          />
        </div>
      )}
    </>
  );
}

export default BasketForm;
