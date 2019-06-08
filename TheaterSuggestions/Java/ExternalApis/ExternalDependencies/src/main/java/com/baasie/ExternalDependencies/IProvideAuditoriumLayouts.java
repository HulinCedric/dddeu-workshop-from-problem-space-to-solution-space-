package com.baasie.ExternalDependencies;

import com.baasie.ExternalDependencies.auditoriumlayoutrepository.AuditoriumDto;

public interface IProvideAuditoriumLayouts {

    AuditoriumDto getAuditoriumSeatingFor(String showId);
}
