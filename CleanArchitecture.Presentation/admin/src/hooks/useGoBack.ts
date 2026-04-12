import { useCallback } from "react";
import { useRouter } from "next/navigation";

const useGoBack = () => {
  const router = useRouter();

  const goBack = useCallback(() => {
    if (window.history.length > 1) {
      router.back();
    } else {
      router.push("/");
    }
  }, [router]);

  return goBack;
};

export default useGoBack;
