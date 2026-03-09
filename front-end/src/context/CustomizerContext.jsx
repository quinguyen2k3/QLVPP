import { createContext, useState, useEffect } from 'react';
import config from './config';
import React from 'react';

export const CustomizerContext = createContext(undefined);

export const CustomizerContextProvider = ({ children }) => {
  const getInitialParam = (key, initialValue) => {
    if (typeof window !== 'undefined') {
      const storedValue = localStorage.getItem(key);
      if (storedValue !== null) {
        if (storedValue === 'true') return true;
        if (storedValue === 'false') return false;
        if (!isNaN(Number(storedValue)) && typeof initialValue === 'number') {
          return Number(storedValue);
        }
        return storedValue;
      }
    }
    return initialValue;
  };

  const [activeDir, setActiveDir] = useState(() => getInitialParam('activeDir', config.activeDir));
  const [activeMode, setActiveMode] = useState(() =>
    getInitialParam('activeMode', config.activeMode),
  );
  const [activeTheme, setActiveTheme] = useState(() =>
    getInitialParam('activeTheme', config.activeTheme),
  );
  const [activeLayout, setActiveLayout] = useState(() =>
    getInitialParam('activeLayout', config.activeLayout),
  );
  const [isCardShadow, setIsCardShadow] = useState(() =>
    getInitialParam('isCardShadow', config.isCardShadow),
  );
  const [isLayout, setIsLayout] = useState(() => getInitialParam('isLayout', config.isLayout));
  const [isBorderRadius, setIsBorderRadius] = useState(() =>
    getInitialParam('isBorderRadius', config.isBorderRadius),
  );
  const [isCollapse, setIsCollapse] = useState(() =>
    getInitialParam('isCollapse', config.isCollapse),
  );
  const [isLanguage, setIsLanguage] = useState(() =>
    getInitialParam('isLanguage', config.isLanguage),
  );

  const [isSidebarHover, setIsSidebarHover] = useState(false);
  const [isMobileSidebar, setIsMobileSidebar] = useState(false);

  useEffect(() => {
    document.documentElement.setAttribute('class', activeMode);
    document.documentElement.setAttribute('dir', activeDir);
    document.documentElement.setAttribute('data-color-theme', activeTheme);
    document.documentElement.setAttribute('data-layout', activeLayout);
    document.documentElement.setAttribute('data-boxed-layout', isLayout);
    document.documentElement.setAttribute('data-sidebar-type', isCollapse);

    localStorage.setItem('activeDir', activeDir);
    localStorage.setItem('activeMode', activeMode);
    localStorage.setItem('activeTheme', activeTheme);
    localStorage.setItem('activeLayout', activeLayout);
    localStorage.setItem('isCardShadow', isCardShadow);
    localStorage.setItem('isLayout', isLayout);
    localStorage.setItem('isBorderRadius', isBorderRadius);
    localStorage.setItem('isCollapse', isCollapse);
    localStorage.setItem('isLanguage', isLanguage);
  }, [
    activeDir,
    activeMode,
    activeTheme,
    activeLayout,
    isCardShadow,
    isLayout,
    isBorderRadius,
    isCollapse,
    isLanguage,
  ]);

  return (
    <CustomizerContext.Provider
      value={{
        activeDir,
        setActiveDir,
        activeMode,
        setActiveMode,
        activeTheme,
        setActiveTheme,
        activeLayout,
        setActiveLayout,
        isCardShadow,
        setIsCardShadow,
        isLayout,
        setIsLayout,
        isBorderRadius,
        setIsBorderRadius,
        isCollapse,
        setIsCollapse,
        isLanguage,
        setIsLanguage,
        isSidebarHover,
        setIsSidebarHover,
        isMobileSidebar,
        setIsMobileSidebar,
      }}
    >
      {children}
    </CustomizerContext.Provider>
  );
};
