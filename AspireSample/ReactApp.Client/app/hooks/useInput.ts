import { useCallback, useEffect, useMemo, useReducer, useState } from "react";

type Validation<T> = {
  validate: (value: T) => boolean;
  errorMessage: (value: T) => string;
}[];

type ValidationResult<T> = {
  isValid: boolean;
  errorMessage: string;
}[];

export default function useInput<T>(
  initialValue: T,
  validation?: Validation<T>
) {
  if (validation && !Array.isArray(validation)) {
    throw new Error("Validation must be an array of validation rules");
  }

  const [value, dispatch] = useReducer(inputReducer, initialValue);


  const validate = useCallback((v: T) =>
    {
        console.info("Input validated: ", v);
        return validation?.map((rule) => {
            const isValid = rule.validate(v);
            return {
                isValid,
                errorMessage: isValid ? "" : rule.errorMessage(v),
            };
        }) ?? [];
    }, []);

  const validationResult = useMemo(() => validate(value), [value, validate]);
  
  function change(v: T) {
    dispatch({ type: "change", value: v });
  }

  function reset() {
    dispatch({ type: "reset" });
  }

  function blur(v: T) {
    dispatch({ type: "blur", value: v });
  }

  return {
    value,
    change,
    reset,
    blur,
    isValid:
      validationResult.length > 0
        ? validationResult.every((result) => result.isValid)
        : true,
    errors: validationResult
      .filter((result) => !result.isValid)
      .map((result) => result.errorMessage),
  };
}

function inputReducer<T>(state: T, action: { type: string; value?: T }): T {
  switch (action.type) {
    case "change":
      return action.value ?? state;
    case "reset":
      return state;
    case "blur":
      return action.value ?? state;
    default:
      throw new Error(`Unknown action: ${action}`);
  }
}
